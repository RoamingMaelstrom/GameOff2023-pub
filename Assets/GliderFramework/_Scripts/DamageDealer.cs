using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] public GameObjectFloatSOEvent despawnEvent;
    [SerializeField] public GameObjectFloatSOEvent dealtDamageEvent;
    [SerializeField] public float damageValue;

    [SerializeField] public float dotDamageValue;
    [SerializeField] float DotInterval; 

    [SerializeField] public float life;

    public bool alive = true;
    public bool invulnerable = false;

    public float despawnFloatValue;


    public GameObject createdBy;

    public DotDamageSystem dotDamageSystem = new DotDamageSystem();

    // Ensures that dotInterval cannot be shorter than physics update time (0.02f).
    public float dotInterval 
    {
        get => DotInterval;
        set => DotInterval = Mathf.Max(Time.fixedDeltaTime, value);
    }

    // DamageDealer will only check for dot 
    private void Start() 
    {
        // if (dotDamageValue > 0) StartCoroutine(DotDamageCoroutine());

        if (dotDamageValue > 0) dotDamageSystem.RestartDotDamageCoroutine(this);
    }

    // Stops DotDamageCoroutine if it is running, and then starts a new DotDamageCoroutine. 
    // Also sets alive to true (otherwise coroutine would immediately stop).
    public void ManuallyRestartDotDamageCoroutine()
    {
        alive = true;
        if (dotDamageValue <= 0) return;
        dotDamageSystem.RestartDotDamageCoroutine(this);
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.TryGetComponent(out Health otherHealth)) return;
        if (damageValue > 0) DealDamage(damageValue, otherHealth);
        
        dotDamageSystem.TrackObjectHealth(otherHealth);
    }

    private void OnTriggerExit2D(Collider2D other) => dotDamageSystem.StopTrackingObjectHealth(other.gameObject.GetInstanceID());

    public void DealDamage(float damageValue, Health healthDamaging)
    {
        if (!alive) return;

        healthDamaging.TakeDamage(damageValue);
        if (dealtDamageEvent) dealtDamageEvent.Invoke(gameObject, damageValue);
        if (invulnerable) return;


        life --;
        if (life > 0) return;

        alive = false;
        despawnEvent.Invoke(gameObject, despawnFloatValue);
    }

    private void OnEnable() 
    {
        ManuallyRestartDotDamageCoroutine();
    }
}


// Maintains the time since an object (with Health Component) took damage from a particular DamageDealer.
// Needed for dot not dealing damage every physics update and for piercing projectiles.
[System.Serializable]
public class ObjectDotInfo
{
    public Health objectHealth;
    public float timeSinceDamaged;

    public ObjectDotInfo(Health _objectHealth, float _timeSinceDamaged)
    {
        objectHealth = _objectHealth;
        timeSinceDamaged = _timeSinceDamaged;
    }

    public ObjectDotInfo(Health _objectHealth)
    {
        objectHealth = _objectHealth;
        timeSinceDamaged = 0f;
    }
}


[System.Serializable]
public class DotDamageSystem
{   
    Coroutine dotCoroutine;
    List<ObjectDotInfo> collidingGameObjectsDotInfo = new List<ObjectDotInfo>();

    public void TrackObjectHealth(Health health) => collidingGameObjectsDotInfo.Add(new ObjectDotInfo(health));

    public void StopTrackingObjectHealth(int healthObjectID)
    {
        int targetIndex = GetColliderDotInfoIndex(healthObjectID);
        if (targetIndex == -1) return;
        collidingGameObjectsDotInfo.RemoveAt(targetIndex); 
    }

    public void StopDotDamageCoroutine(MonoBehaviour damageDealer)
    {
        if (dotCoroutine != null) damageDealer.StopCoroutine(dotCoroutine);
        collidingGameObjectsDotInfo.Clear();   
    }

    public void RestartDotDamageCoroutine(DamageDealer damageDealer)
    {
        StopDotDamageCoroutine(damageDealer);
        dotCoroutine = damageDealer.StartCoroutine(DotDamageCoroutine(damageDealer));
    }

    IEnumerator DotDamageCoroutine(DamageDealer damageDealer)
    {
        while (true)
        {
            for (int i = collidingGameObjectsDotInfo.Count - 1; i >=0; i--)
            {
                ObjectDotInfo dotItem = collidingGameObjectsDotInfo[i];
                dotItem.timeSinceDamaged -= Time.fixedDeltaTime;
                if (dotItem.timeSinceDamaged <= 0) 
                {
                    damageDealer.DealDamage(damageDealer.dotDamageValue, dotItem.objectHealth);
                    dotItem.timeSinceDamaged += damageDealer.dotInterval;
                }

            }

            yield return new WaitForFixedUpdate();
        }
    }

    private int GetColliderDotInfoIndex(int colliderID)
    {
        for (int i = 0; i < collidingGameObjectsDotInfo.Count; i++)
        {
            if (collidingGameObjectsDotInfo[i].objectHealth == null) return i;
            if (collidingGameObjectsDotInfo[i].objectHealth.gameObject.GetInstanceID() == colliderID) return i;
        }
        return -1;
    }

}
