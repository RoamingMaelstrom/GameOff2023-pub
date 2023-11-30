using SOEvents;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent fireWeaponEvent;
    [SerializeField] public int scale;
    [SerializeField] public int groupID;
    [SerializeField] Health health;
    [SerializeField] public Rigidbody2D body;
    public EnemyFlags flags;

    [Header("State Transition Distances")]
    public float enterPursuitDistance = 200f;
    public float exitPursuitDistance = 250f;
    public float enterCombatDistance = 25f;
    public float exitCombatDistance = 40f;
    public float farFromGroupDistance = 20f;

    [Header("Movement Variables")]
    public float acceleration;
    public float standardSpeed;
    public float combatSpeed;
    public float runningSpeed;
    public float maxRotationSpeed;
    public float maxTorque;
    public float maxNodeDistance;
    public float minNodeDistance;

    [Header("Combat Variables")]
    public float minBurstCooldown= 1f;
    public float maxBurstCooldown = 3f;
    public float minFiringDuration = 0.4f;
    public float maxFiringDuration = 0.8f;
    public float weaponFireRate = 4f;
    public WeaponType weaponType;
    public int munitionID;
    public float munitionSpeed;

    [Header("Running State Variables")]
    public float runningHpPercent = 0.15f;
    public float runningSpeedPercent = 0.5f;

    [Header("Set By EnemyStateManager")]
    public EnemyStateType stateType;
    public EnemyStateType previousStateType;
    public Vector3 moveToPos;
    public Vector3 targetPos;
    public Vector3 nearestEnemyPos;

    public float timer;
    public float timeSinceStateChange;
    public float combatCountdown;


    public void UpdateTimers(float deltaTime)
    {
        timer += deltaTime;
        timeSinceStateChange += deltaTime;
        combatCountdown -= deltaTime;
    }

    private void FixedUpdate() 
    {
        HandleThrust();
        HandleRotation();
        UpdateTimers(Time.fixedDeltaTime);
        HandleWeapons();
    }

    private void HandleWeapons()
    {
        if (stateType != EnemyStateType.COMBAT) return;
        if (combatCountdown < -5) combatCountdown = Random.Range(minBurstCooldown / 5f, minBurstCooldown / 3f);
        if (combatCountdown < 0) 
        {
            fireWeaponEvent.Invoke(gameObject);
            combatCountdown = Random.Range(minBurstCooldown, maxBurstCooldown);
        }
    }

    public void UpdateFlags()
    {
        int flag = 0;
        flag += SetLowHealthFlag();
        flag += SetInFiringRangeFlag();
        flag += SetInPursuitRangeFlag();

        flags = (EnemyFlags)flag;
    }

    private void HandleThrust()
    {
        float maxSpeed;
        switch(stateType)
        {
            case EnemyStateType.COMBAT: maxSpeed = combatSpeed; break;
            case EnemyStateType.RUNNING: maxSpeed = runningSpeed; break;
            default: maxSpeed = standardSpeed; break;
        }

        Vector2 thrustDirectionVector = (moveToPos - transform.position).normalized;
        body.AddForce(thrustDirectionVector * acceleration * body.mass);
        body.velocity = body.velocity.normalized * Mathf.Min(Mathf.Abs(body.velocity.magnitude), maxSpeed);
    }

    private void HandleRotation()
    {
        float currentRotation = body.transform.rotation.eulerAngles.z;
        float targetRotation = GetTargetRotation();
        float deltaRotation = targetRotation - currentRotation;

        SetTorque(deltaRotation, GetWorkingTorqueValue(deltaRotation, maxTorque, maxRotationSpeed));
        DampenWobble(deltaRotation, body);
        body.angularVelocity = Mathf.Clamp(body.angularVelocity, -maxRotationSpeed, maxRotationSpeed);
    }

    private float GetTargetRotation()
    {
        Vector2 rotateAround;
        if (stateType == EnemyStateType.COMBAT) rotateAround = targetPos;
        else rotateAround = body.transform.position + (Vector3)(body.velocity * 1000f);

        float targetRotation = - Mathf.Atan2(rotateAround.x - body.transform.position.x, rotateAround.y - body.transform.position.y) * Mathf.Rad2Deg;
        return (targetRotation < 0) ? targetRotation + 360f : targetRotation;
    }

    private void SetTorque(float deltaRotation, float torqueValue)
    {
        if (deltaRotation < - 180 || (deltaRotation > 0 && deltaRotation < 180)) body.AddTorque(torqueValue);
        else body.AddTorque(-torqueValue);
    }

    // Allows more precise rotation when deltaRotation is low.
    private float GetWorkingTorqueValue(float deltaRotation, float maxTorque, float maxAngularVelocity) 
    {
        float maxAngularVelocityFixedUpdate = maxAngularVelocity * Time.fixedDeltaTime;
        return Mathf.Clamp((Mathf.Abs(deltaRotation) - maxAngularVelocityFixedUpdate) * maxTorque / maxAngularVelocityFixedUpdate, 0, maxTorque);
    }

    private void DampenWobble(float deltaRotation, Rigidbody2D enemyBody)
    {
        float absDeltaRotation = Mathf.Abs(deltaRotation);
        for (int i = 4; i >= 1; i/=2)
        {
            if (i > absDeltaRotation) enemyBody.angularVelocity = Mathf.Lerp(enemyBody.angularVelocity, 0, (8 - i) / 8f);
        }
    }

    private int SetLowHealthFlag() => health.GetCurrentHp() / health.maxHp < runningHpPercent ? (int)EnemyFlags.LOW_HEALTH : 0;

    private int SetInFiringRangeFlag()
    {
        float distance = (transform.position - nearestEnemyPos).magnitude;
        if (stateType == EnemyStateType.COMBAT) return distance < exitCombatDistance ? (int)EnemyFlags.IN_FIRING_RANGE : 0;
        return distance < enterCombatDistance ? (int)EnemyFlags.IN_FIRING_RANGE : 0;
    }

    private int SetInPursuitRangeFlag()
    {
        float distance = (transform.position - nearestEnemyPos).magnitude;
        if ((flags & EnemyFlags.IN_PURSUIT_RANGE) > 0) return distance < exitPursuitDistance ? (int)EnemyFlags.IN_PURSUIT_RANGE : 0;
        return distance < enterPursuitDistance ? (int)EnemyFlags.IN_PURSUIT_RANGE : 0;
    }
}



public enum EnemyFlags
{
    _NOTHING = 0,
    LOW_HEALTH = 1,
    IN_FIRING_RANGE = 2,
    IN_PURSUIT_RANGE = 4
}



public enum EnemyStateType
{
    IDLE, 
    PURSUIT, 
    COMBAT,
    RUNNING
}



