using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageIntervalUpgrader : UpgradeApplier
{
    [SerializeField] DamageDealer playerDamageDealer;
    public override void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel)
    {
        playerDamageDealer.dotInterval *= upgradeInfo.GetUpgradeMagnitude(upgradeLevel);
    }

    public override void RunOnAwake(){}
}
