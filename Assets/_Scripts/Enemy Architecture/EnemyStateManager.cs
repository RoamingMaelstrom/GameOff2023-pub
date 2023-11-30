using System.Collections.Generic;
using System.Diagnostics;
using SOEvents;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent objectSpawnedEvent;
    [SerializeField] GameObjectSOEvent objectReturnedEvent;

    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] BoidContainer boidContainer;
    // Todo: Are these even used (no asteroid dodging implemented so far...) Could be easy performance win to remove these
    [SerializeField] List<AdditionalObjectInfo> objectsInGame = new();
    [SerializeField] List<int> objectsInGameIds = new();
    [SerializeField] List<EnemyAI> enemies = new();
    [SerializeField] List<Turret> turrets = new();
    [SerializeField] string[] enterCombatSfx;

    [Header("Modify Enemy Variables")]
    public float enemyCloseDistance;


    private void Awake() 
    {
        objectSpawnedEvent.AddListener(TryRegisterObject);
        objectReturnedEvent.AddListener(TryUnregisterObject);
    }

    private void FixedUpdate() 
    {
        foreach (EnemyAI enemy in enemies) UpdateEnemy(enemy);
        foreach (Turret turret in turrets) UpdateTurret(turret);
    }

    private void UpdateEnemy(EnemyAI enemy)
    {
        if (enemy.scale > ScaleManager.PlayerScaleGlobal)
        {
            if (enemy.stateType != EnemyStateType.IDLE) SwitchState(enemy, EnemyStateType.IDLE);
            UpdateEnemyVariables(enemy);
            return;
        }
        UpdateEnemyVariables(enemy);
        enemy.UpdateFlags();
        EnemyStateType newState = GetUpdatedEnemyState(enemy);
        if (newState != enemy.stateType) SwitchState(enemy, newState);
    }

    private void UpdateTurret(Turret turret)
    {
        turret.nearestTarget = GetNearestPlayerOrBoidPos(turret.body.transform.position, turret.scale);
    }

    private void UpdateEnemyVariables(EnemyAI enemy)
    {
        enemy.nearestEnemyPos = GetNearestPlayerOrBoidPos(enemy.body.transform.position, enemy.scale);
        if (enemy.stateType == EnemyStateType.COMBAT || enemy.stateType == EnemyStateType.PURSUIT) enemy.targetPos = GenerateNewTargetPos(enemy);

        float distanceToNode = (enemy.body.transform.position - enemy.moveToPos).magnitude;
        if (distanceToNode < enemy.minNodeDistance || distanceToNode > enemy.maxNodeDistance * 1.1f) enemy.moveToPos = GenerateNewMoveToPos(enemy);
    }

    private Vector3 GetNearestPlayerOrBoidPos(Vector3 position, int scale)
    {
        Vector3 closestPos = playerBody.transform.position;
        float distanceToNearestSqr = (closestPos - position).sqrMagnitude;

        float boidDistanceSqr;
        foreach (var chain in boidContainer.boidChains)
        {
            Boid boid = chain.GetBoid();
            if (boid == null) continue;
            if (boid.numberOfDrones == 0) continue;
            if (boid.scale + 1 < scale) continue;

            boidDistanceSqr = ((Vector2)position - boid.centre).sqrMagnitude;
            if (boidDistanceSqr < distanceToNearestSqr)
            {
                closestPos = boid.centre;
                distanceToNearestSqr = boidDistanceSqr;
            }
        }

        return closestPos;
    }

    private Vector3 GenerateNewMoveToPos(EnemyAI enemy)
    {
        Vector3 enemyPos = enemy.body.transform.position;
        switch (enemy.stateType)
        {
            case EnemyStateType.PURSUIT:  
            {
                Vector2 deltaPos = enemy.targetPos - enemyPos;
                float distance = deltaPos.magnitude;
                return enemyPos + ((Vector3)deltaPos.normalized * Mathf.Min(Random.Range(enemy.maxNodeDistance / 3f, enemy.maxNodeDistance), distance));
            }
            case EnemyStateType.COMBAT: return enemy.targetPos + ((Vector3)Random.insideUnitCircle.normalized * Random.Range(enemy.enterCombatDistance / 2f, enemy.enterCombatDistance));
            case EnemyStateType.RUNNING: return enemy.moveToPos = enemyPos + (enemy.moveToPos - enemyPos).normalized * enemy.maxNodeDistance; 
            default: return enemyPos + ((Vector3)Random.insideUnitCircle.normalized * Random.Range(enemy.maxNodeDistance / 2f, enemy.maxNodeDistance)) + ((playerBody.transform.position - enemyPos).normalized * enemy.maxNodeDistance * (ScaleManager.PlayerScaleGlobal < enemy.scale ? 0.008f : 0.08f));
        }
    }

    private Vector3 GenerateNewTargetPos(EnemyAI enemy)
    {
        // Todo: Bit strange, but will keep using playerVelocity even on targeting boid groups for now
        return enemy.nearestEnemyPos + ((Vector3)playerBody.velocity * 0.25f);
    }

    private EnemyStateType GetUpdatedEnemyState(EnemyAI enemy)
    {
        //if ((enemy.flags & EnemyFlags.LOW_HEALTH) == EnemyFlags.LOW_HEALTH) return EnemyStateType.RUNNING;

        switch(enemy.stateType)
        {
            case EnemyStateType.IDLE: if ((enemy.flags & EnemyFlags.IN_PURSUIT_RANGE) == EnemyFlags.IN_PURSUIT_RANGE) return EnemyStateType.PURSUIT; break;
            case EnemyStateType.PURSUIT:
            {
                if ((enemy.flags & EnemyFlags.IN_FIRING_RANGE) == EnemyFlags.IN_FIRING_RANGE) return EnemyStateType.COMBAT;
                if ((enemy.flags & EnemyFlags.IN_PURSUIT_RANGE) == 0) return EnemyStateType.IDLE;
                break;
            }
            case EnemyStateType.COMBAT: if ((enemy.flags & EnemyFlags.IN_FIRING_RANGE) == 0) return EnemyStateType.PURSUIT; break;
        }
        return enemy.stateType;
    }

    private void SwitchState(EnemyAI enemy, EnemyStateType newState)
    {
        enemy.previousStateType = enemy.stateType;
        enemy.stateType = newState;
        enemy.timeSinceStateChange = 0;

        if (newState == EnemyStateType.COMBAT && enemy.previousStateType != EnemyStateType.COMBAT) GliderSFX.Play.RandomAtPoint(enemy.body.transform.position, enterCombatSfx);

        enemy.targetPos = GenerateNewTargetPos(enemy);
        enemy.moveToPos = GenerateNewMoveToPos(enemy);
    }

    public void TryRegisterObject(GameObject newObject)
    {
        if (newObject.TryGetComponent(out AdditionalObjectInfo objectInfo))
        {
            objectsInGame.Add(objectInfo);
            objectsInGameIds.Add(newObject.GetInstanceID());

            if (newObject.TryGetComponent(out EnemyAI aiModule)) 
            {
                enemies.Add(aiModule);
                if (aiModule.stateType != EnemyStateType.IDLE) SwitchState(aiModule, EnemyStateType.IDLE);
                aiModule.moveToPos = GenerateNewMoveToPos(aiModule);
                SetEnemySpawnOutsidePursuit(aiModule);
                UpdateEnemy(aiModule);
            }

            if (newObject.TryGetComponent(out Turret turretModule)) 
            {
                turrets.Add(turretModule);
                UpdateTurret(turretModule);
            }

            return;
        }
    }

    private void SetEnemySpawnOutsidePursuit(EnemyAI aiModule)
    {
        if (aiModule.gameObject.layer == LayerMask.NameToLayer("no_interactions")) return;
        if ((aiModule.body.transform.position - playerBody.transform.position).magnitude < aiModule.enterPursuitDistance)
        {
            aiModule.body.transform.position = playerBody.transform.position + (Vector3)(Random.insideUnitCircle.normalized * aiModule.exitPursuitDistance * Random.Range(1f, 1.5f));
        }
    }

    public void TryUnregisterObject(GameObject newObject)
    {
        if (newObject.TryGetComponent(out AdditionalObjectInfo objectInfo))
        {
            if (newObject.TryGetComponent(out EnemyAI aiModule)) enemies.Remove(aiModule);
            if (newObject.TryGetComponent(out Turret turretModule)) turrets.Remove(turretModule);

            int index = objectsInGameIds.FindIndex(objectID => objectID == newObject.GetInstanceID());
            if (index == -1) return;

            objectsInGame.RemoveAt(index);
            objectsInGameIds.RemoveAt(index);
            return;
        }
    }
}
