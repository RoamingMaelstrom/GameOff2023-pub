using System.Collections;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class MunitionManager : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent fireWeaponEvent;
    [SerializeField] ObjectPoolMain objectPoolMain;
    [SerializeField] float alternateDistanceSize = 1f;
    [SerializeField] [Range(0.1f, 1f)] float tripleShotCooldownMultiplier = 0.4f;

    [SerializeField] string[] enemyLaserSfx;
    [SerializeField] string[] turretLaserSfx;
    [SerializeField] string[] drone2LaserSfx;
    [SerializeField] string[] drone5LaserSfx;

    private void Awake() 
    {
        fireWeaponEvent.AddListener(HandleWeaponFiring);
    }

    private void HandleWeaponFiring(GameObject munitionCreator)
    {
        if (munitionCreator.TryGetComponent(out EnemyAI enemyAI))
        {
            HandleEnemyWeaponFire(enemyAI);
            return;
        }

        if (munitionCreator.TryGetComponent(out Turret turretModule))
        {
            HandleTurretWeaponFire(turretModule);
            return;
        }

        if (munitionCreator.TryGetComponent(out Boid boid))
        {
            HandleBoidWeaponFire(boid);
            return;
        }

    }

    private void HandleBoidWeaponFire(Boid boid)
    {
        Rigidbody2D randomDrone = boid.droneList[Random.Range(0, boid.droneList.Count)];
        FiringOrderInfo fireOrderInfo = new FiringOrderInfo
        {
            weaponFireRate = boid.boidFiringInfo.fireRatePerDrone,
            minFiringDuration = 0.02f,
            maxFiringDuration = 0.04f,
            accuracyCoefficient = 0.65f,
            parentObject = randomDrone.gameObject,
            munitionID = boid.boidFiringInfo.munitionID,
            munitionSpeed = boid.boidFiringInfo.munitionSpeed,
            startPos = randomDrone.transform.position,
            targetPos = boid.rangedTargetPos,
            parentVelocity = randomDrone.velocity,
            fireOrderCreatorType = boid.droneID < 30300 ? FireOrderCreatorType.DRONE_2 : FireOrderCreatorType.DRONE_5
        };

        switch (boid.boidFiringInfo.weaponType)
        {
            case WeaponType.ALTERNATING: StartCoroutine(AlternatingShot(fireOrderInfo)); break;
            case WeaponType.DOUBLE_WIDTH: StartCoroutine(DoubleWidthShot(fireOrderInfo)); break;
            case WeaponType.TRIPLE: StartCoroutine(TripleShot(fireOrderInfo)); break;
            case WeaponType.SINGLE: StartCoroutine(SingleShot(fireOrderInfo)); break;
            default: return;
        }
    }

    private void HandleTurretWeaponFire(Turret turret)
    {
        FiringOrderInfo fireOrderInfo = new FiringOrderInfo
        {
            weaponFireRate = turret.weaponFireRate,
            accuracyCoefficient = 0.4f,
            minFiringDuration = turret.minFiringDuration,
            maxFiringDuration = turret.maxFiringDuration,
            parentObject = turret.gameObject,
            munitionID = turret.munitionID,
            munitionSpeed = turret.munitionSpeed,
            startPos = turret.body.transform.position,
            targetPos = turret.nearestTarget,
            parentVelocity = turret.body.velocity,
            fireOrderCreatorType = FireOrderCreatorType.TURRET
        };

        switch (turret.weaponType)
        {
            case WeaponType.ALTERNATING: StartCoroutine(AlternatingShot(fireOrderInfo)); break;
            case WeaponType.DOUBLE_WIDTH: StartCoroutine(DoubleWidthShot(fireOrderInfo)); break;
            case WeaponType.TRIPLE: StartCoroutine(TripleShot(fireOrderInfo)); break;
            case WeaponType.SINGLE: StartCoroutine(SingleShot(fireOrderInfo)); break;
            default: return;
        }
    }

    private void HandleEnemyWeaponFire(EnemyAI enemyAI)
    {
        FiringOrderInfo fireOrderInfo = new FiringOrderInfo
        {
            weaponFireRate = enemyAI.weaponFireRate,
            accuracyCoefficient = 0.4f,
            minFiringDuration = enemyAI.minFiringDuration,
            maxFiringDuration = enemyAI.maxFiringDuration,
            parentObject = enemyAI.gameObject,
            munitionID = enemyAI.munitionID,
            munitionSpeed = enemyAI.munitionSpeed,
            startPos = enemyAI.body.transform.position,
            targetPos = enemyAI.targetPos,
            parentVelocity = enemyAI.body.velocity,
            fireOrderCreatorType = FireOrderCreatorType.ENEMY          
        };

        switch (enemyAI.weaponType)
        {
            case WeaponType.ALTERNATING: StartCoroutine(AlternatingShot(fireOrderInfo)); break;
            case WeaponType.DOUBLE_WIDTH: StartCoroutine(DoubleWidthShot(fireOrderInfo)); break;
            case WeaponType.TRIPLE: StartCoroutine(TripleShot(fireOrderInfo)); break;
            case WeaponType.SINGLE: StartCoroutine(SingleShot(fireOrderInfo)); break;
            default: return;
        }
    }


    private GameObject CreateMunition(FiringOrderInfo fireOrderInfo, Vector3 startOffset)
    {
        GameObject munitionObject = objectPoolMain.GetObject(fireOrderInfo.munitionID);
        Rigidbody2D munitionBody = munitionObject.GetComponent<Rigidbody2D>();
        Vector2 dirVectorToTarget = WeaponMath.Math.GetDirectionVectorToTarget(fireOrderInfo.startPos, fireOrderInfo.targetPos);
        munitionBody.velocity = dirVectorToTarget * fireOrderInfo.munitionSpeed;
        munitionBody.velocity += WeaponMath.Math.ApplyAccuracyCoefficientToVector(munitionBody.velocity, 0.65f);

        munitionObject.transform.position = fireOrderInfo.parentObject.transform.position + startOffset;
        munitionObject.transform.localRotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(munitionBody.velocity.normalized));

        DamageDealer damageDealer = munitionObject.GetComponent<DamageDealer>();
        damageDealer.alive = true;
        damageDealer.life = 1;

        switch (fireOrderInfo.fireOrderCreatorType)
        {
            case FireOrderCreatorType.ENEMY: GliderSFX.Play.RandomAtPoint(fireOrderInfo.parentObject.transform.position, enemyLaserSfx); break;
            case FireOrderCreatorType.TURRET: GliderSFX.Play.RandomAtPoint(fireOrderInfo.parentObject.transform.position, turretLaserSfx); break;
            case FireOrderCreatorType.DRONE_2: GliderSFX.Play.RandomAtPoint(fireOrderInfo.parentObject.transform.position, drone2LaserSfx); break;
            case FireOrderCreatorType.DRONE_5: GliderSFX.Play.RandomAtPoint(fireOrderInfo.parentObject.transform.position, drone5LaserSfx); break;
            default: break;
        }

        return munitionObject;
    }

    private IEnumerator SingleShot(FiringOrderInfo fireOrderInfo)
    {
        float firingDuration = Random.Range(fireOrderInfo.minFiringDuration, fireOrderInfo.maxFiringDuration);
        float fireCooldown = 1f / fireOrderInfo.weaponFireRate;
        int counter = 0;
        while (firingDuration > 0 && counter < 100)
        {
            counter ++;
            if (!fireOrderInfo.parentObject.activeInHierarchy) break;
            firingDuration -= fireCooldown;
            CreateMunition(fireOrderInfo, Vector3.zero);
            yield return new WaitForSeconds(fireCooldown);
        }
    }

    private IEnumerator AlternatingShot(FiringOrderInfo fireOrderInfo)
    {
        float firingDuration = Random.Range(fireOrderInfo.minFiringDuration, fireOrderInfo.maxFiringDuration);
        float fireCooldown = 1f / fireOrderInfo.weaponFireRate;
        float enemyRotation;
        Vector3 offset;
        float alternate = 1f;
        int counter = 0;
        while (firingDuration > 0 && counter < 100)
        {          
            counter ++;  
            if (!fireOrderInfo.parentObject.activeInHierarchy) break;
            enemyRotation = fireOrderInfo.parentObject.transform.localEulerAngles.z;
            offset = new Vector3(Mathf.Cos(enemyRotation * Mathf.Deg2Rad), Mathf.Sin(enemyRotation * Mathf.Deg2Rad), 0) * alternate * alternateDistanceSize;
            firingDuration -= fireCooldown;
            CreateMunition(fireOrderInfo, offset);
            alternate *= -1;
            yield return new WaitForSeconds(fireCooldown);
        }
    }

    private IEnumerator DoubleWidthShot(FiringOrderInfo fireOrderInfo)
    {
        float firingDuration = Random.Range(fireOrderInfo.minFiringDuration, fireOrderInfo.maxFiringDuration);
        float fireCooldown = 1f / fireOrderInfo.weaponFireRate;
        float enemyRotation;
        Vector3 offset;
        int counter = 0;
        while (firingDuration > 0 && counter < 100)
        {
            counter ++;
            if (!fireOrderInfo.parentObject.activeInHierarchy) break;
            enemyRotation = fireOrderInfo.parentObject.transform.localEulerAngles.z;
            offset = new Vector3(Mathf.Cos(enemyRotation * Mathf.Deg2Rad), Mathf.Sin(enemyRotation * Mathf.Deg2Rad), 0) * alternateDistanceSize / 2f;
            firingDuration -= fireCooldown;
            CreateMunition(fireOrderInfo, offset);
            CreateMunition(fireOrderInfo, - offset);
            yield return new WaitForSeconds(fireCooldown);
        }
    }

    private IEnumerator TripleShot(FiringOrderInfo fireOrderInfo)
    {
        float firingDuration = Random.Range(fireOrderInfo.minFiringDuration, fireOrderInfo.maxFiringDuration);
        float fireCooldown = 1f / fireOrderInfo.weaponFireRate;
        int counter = 0;
        int antiInfCounter = 0;
        while (firingDuration > 0 && antiInfCounter < 100)
        {
            antiInfCounter ++;
            if (!fireOrderInfo.parentObject.activeInHierarchy) break;
            CreateMunition(fireOrderInfo, Vector3.zero);
            counter ++;
            firingDuration -= counter % 3 == 0 ? fireCooldown * 1.5f : 0;
            yield return new WaitForSeconds(counter % 3 == 0 ? fireCooldown: fireCooldown * tripleShotCooldownMultiplier);
        }
    }
}

public struct FiringOrderInfo
{   
    public GameObject parentObject;
    public int munitionID;
    public float munitionSpeed;
    public float accuracyCoefficient;
    public Vector3 startPos;
    public Vector3 targetPos;
    public Vector2 parentVelocity;

    public float weaponFireRate;
    public float minFiringDuration;
    public float maxFiringDuration;
    public FireOrderCreatorType fireOrderCreatorType;
}

public enum FireOrderCreatorType
{
    ENEMY,
    TURRET,
    DRONE_2,
    DRONE_5
}
