using UnityEngine;

public class PlayerSpeedUpgradeApplier : UpgradeApplier
{
    [SerializeField] BasePlayerController basePlayerController;
    public override void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel)
    {
        basePlayerController.AddUpgradeMaxSpeedModifier(upgradeInfo.GetUpgradeMagnitude(upgradeLevel));
    }

    public override void RunOnAwake(){}
}
