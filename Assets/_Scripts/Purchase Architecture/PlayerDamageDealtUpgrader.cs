using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageDealtUpgrader : UpgradeApplier
{
    [SerializeField] DamageDealer playerDamageDealer;
    public override void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel)
    {
        playerDamageDealer.dotDamageValue += upgradeInfo.GetUpgradeMagnitude(upgradeLevel) * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1);
    }

    public override void RunOnAwake(){}
}
