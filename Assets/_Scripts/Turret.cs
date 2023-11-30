using System.Collections;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent fireWeaponEvent;
    [SerializeField] public Rigidbody2D body;
    public int scale = 4;
    public Vector3 nearestTarget;
    public float maxRotationSpeed = 120f;
    public float maxTorque = 75f;
    
    [Header("Combat Variables")]
    public float fireRadius;
    public float minBurstCooldown = 1f;
    public float maxBurstCooldown = 3f;
    public float minFiringDuration = 0.4f;
    public float maxFiringDuration = 0.8f;
    public float weaponFireRate = 4f;
    public WeaponType weaponType;
    public int munitionID;
    public float munitionSpeed;

    public float combatCountdown;
    public bool firing = false;


    private void FixedUpdate() 
    {
        HandleRotation();
        HandleFiring();

    }

    private void HandleFiring()
    {
        combatCountdown -= Time.fixedDeltaTime;
        if (combatCountdown < -5) combatCountdown = Random.Range(minBurstCooldown / 5f, minBurstCooldown / 3f);
        if (ScaleManager.PlayerScaleGlobal < scale) return;
        firing = (nearestTarget - body.transform.position).magnitude < fireRadius;
        if (!firing) return;

        if (combatCountdown < 0) 
        {
            fireWeaponEvent.Invoke(gameObject);
            combatCountdown = Random.Range(minBurstCooldown, maxBurstCooldown);
        }
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
        Vector2 rotateAround = nearestTarget;

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

}
