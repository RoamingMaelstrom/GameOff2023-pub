using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupRangeUpgrader : UpgradeApplier
{
    [SerializeField] CircleCollider2D playerPickupCollider;
    public override void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel)
    {
        playerPickupCollider.radius += upgradeInfo.GetUpgradeMagnitude(upgradeLevel);
    }

    public override void RunOnAwake(){}
}
