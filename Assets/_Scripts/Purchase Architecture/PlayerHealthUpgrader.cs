using UnityEngine;

public class PlayerHealthUpgrader : UpgradeApplier
{
    [SerializeField] Health playerHealth;
    public override void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel)
    {
        playerHealth.damageTakeMultiplier *= upgradeInfo.GetUpgradeMagnitude(upgradeLevel);
    }

    public override void RunOnAwake(){}
}
