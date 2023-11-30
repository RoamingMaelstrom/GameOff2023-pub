using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInfoSO", menuName = "UpgradeInfoSO", order = 5)]
public class UpgradeInfo : ScriptableObject
{
    [field: SerializeField] public int upgradeID {get; private set;}
    public string upgradeName;
    public List<int> upgradeCostPerLevel = new();
    public List<float> upgradeMagnitudePerLevel = new();
    public UpgradePanelDisplaySignInfo upgradePanelDisplaySignInfo;
    public string unit;

    public int GetUpgradeCost(int level) => level <= upgradeCostPerLevel.Count ? upgradeCostPerLevel[level - 1] : -1;
    public float GetUpgradeMagnitude(int level) => level <= upgradeMagnitudePerLevel.Count ? upgradeMagnitudePerLevel[level - 1] : 0;
    public bool IsMaxLevel(int level) => level > upgradeCostPerLevel.Count;
}

[System.Serializable]
public class InstanceUpgradeInfo
{
    public int upgradeID;
    public UpgradeInfo upgradeInfo;
    public int upgradeLevel;

    public InstanceUpgradeInfo(UpgradeInfo upgradeInfo)
    {
        upgradeID = upgradeInfo.upgradeID;
        this.upgradeInfo = upgradeInfo;
        upgradeLevel = 1;
    }

    public int GetCurrentUpgradeCost() => upgradeInfo.GetUpgradeCost(upgradeLevel);
    public bool IsMaxLevel() => upgradeInfo.IsMaxLevel(upgradeLevel);

}

public enum UpgradePanelDisplaySignInfo
{
    PERCENT,
    ONE_MINUS_PERCENT,
    ABSOLUTE_10_SCALING,
    ABSOLUTE_NEGATIVE_SCALING,
    ABSOLUTE_SCALING
}
