using UnityEngine;
using SOEvents;

public class Health : MonoBehaviour
{
    [SerializeField] public GameObjectFloatSOEvent onTakeDamageEvent;
    [SerializeField] public GameObjectFloatSOEvent onDeathEvent;
    [Tooltip("GameObject that is passed to \"onDeathEvent\" when invoked. If none is provided, defaults to parent of GameObject that contains Health Script.")]
    public GameObject defaultGameObjectOnDeath;
    [Tooltip("Float that is passed to \"onDeathEvent\" when invoked.")]
    public float defaultFloatOnDeath;

    [Space(25)]

    [SerializeField] public float maxHp;
    [SerializeField] float currentHp;
    [SerializeField] public float hpRegenRate;
    public float damageTakeMultiplier = 1f;

    [Space(25)]

    [SerializeField] public float shieldHpMax;
    [SerializeField] float shieldHpCurrent;
    [SerializeField] public float shieldRegenRate;
    [SerializeField] public float shieldOnDamagedRegenCooldown;
    [SerializeField] float currentShieldRegenCooldown;

    public float GetCurrentHp() => currentHp;
    public float GetCurrentShieldHp() => shieldHpCurrent;
    public float ManualSetCurrentHp(float value) => currentHp = Mathf.Clamp(value, 0, maxHp);
    public float ManualSetCurrentShieldHp(float value) => shieldHpCurrent = Mathf.Clamp(value, 0, shieldHpMax);
    public float FullHeal() => currentHp = maxHp; 

    // When taking damage, first damages the shield, and puts the shield's regeneration on cooldown. The, subtracts any remaining damage from currentHp.
    public void TakeDamage(float dmgValue)
    {
        shieldHpCurrent -= dmgValue * damageTakeMultiplier;
        currentShieldRegenCooldown = shieldOnDamagedRegenCooldown;

        if (onTakeDamageEvent) onTakeDamageEvent.Invoke(gameObject, dmgValue * damageTakeMultiplier);

        if (shieldHpCurrent < 0)
        {
            currentHp += shieldHpCurrent;
            shieldHpCurrent = 0;
            CheckDead();
        }

    }

    private void FixedUpdate() 
    {
        ApplyRegen();
    }

    void ApplyRegen()
    {
        ManualSetCurrentHp(currentHp + (hpRegenRate * Time.fixedDeltaTime));

        currentShieldRegenCooldown -= Time.fixedDeltaTime;
        if (currentShieldRegenCooldown <= 0) ManualSetCurrentShieldHp(shieldHpCurrent + (shieldRegenRate * Time.fixedDeltaTime)); 
    }

    // Passes parent GameObject into event by default. Can be overriden for different behaviour.
    public void CheckDead() 
    {
        if (currentHp > 0) return;
        if (onDeathEvent != null) onDeathEvent.Invoke(GetDeathGameObjectValue(), GetDeathFloatValue());
        else Debug.Log("onDeathEvent not assigned.");
    }

    public virtual float GetDeathFloatValue()
    {
        return defaultFloatOnDeath;
    }

    public virtual GameObject GetDeathGameObjectValue()
    {
        if (!defaultGameObjectOnDeath) return gameObject;
        return defaultGameObjectOnDeath;
    }
}
