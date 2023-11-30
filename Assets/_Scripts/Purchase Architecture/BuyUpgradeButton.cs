using UnityEngine;
using SOEvents;
using System;
using TMPro;
using UnityEngine.UI;

public class BuyUpgradeButton : MonoBehaviour
{
    [SerializeField] IntSOEvent requestPurchaseEvent;
    [SerializeField] IntSOEvent purchaseCompleteEvent;
    [SerializeField] InstanceUpgradeInfoSOEvent getUpgradeInfoEvent;
    [SerializeField] IntSOEvent upgradeDroneEvent;
    [SerializeField] ScoreLogic scoreLogic;

    [SerializeField] TextMeshProUGUI upgradeNameText;
    [SerializeField] TextMeshProUGUI upgradeMagnitudeText;
    [SerializeField] TextMeshProUGUI upgradeCostText;
    [SerializeField] Button buyButton;


    [SerializeField] int purchaseID;
    [SerializeField] InstanceUpgradeInfo instanceUpgradeInfo;

    private void Awake() 
    {
        requestPurchaseEvent.AddListener(UpdateFields);
        upgradeDroneEvent.AddListener(UpdateFields);
    }

    private void Start() 
    {
        instanceUpgradeInfo.upgradeID = purchaseID;
        GetUpdatedInstanceUpgradeInfo();
    }

    private void OnEnable() 
    {
        instanceUpgradeInfo.upgradeID = purchaseID;
        GetUpdatedInstanceUpgradeInfo();
    }

    public void RequestPurchase()
    {
        requestPurchaseEvent.Invoke(purchaseID);
        GetUpdatedInstanceUpgradeInfo();
    }

    public void GetUpdatedInstanceUpgradeInfo()
    {
        getUpgradeInfoEvent.Invoke(instanceUpgradeInfo);
        UpdateFields();
    }

    private void UpdateFields(int arg0)
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        if (instanceUpgradeInfo == null) 
        {
            buyButton.interactable = false;
            return;
        }
        if (instanceUpgradeInfo.upgradeInfo == null) 
        {
            buyButton.interactable = false;
            return;
        }
        if (instanceUpgradeInfo.IsMaxLevel()) 
        {   
            buyButton.interactable = false;
            SetMaxLevelText();
            return;
        }

        upgradeNameText.SetText(instanceUpgradeInfo.upgradeInfo.upgradeName);
        SetMagnitudeText(instanceUpgradeInfo.upgradeInfo.GetUpgradeMagnitude(instanceUpgradeInfo.upgradeLevel), instanceUpgradeInfo.upgradeInfo.upgradePanelDisplaySignInfo, instanceUpgradeInfo.upgradeInfo.unit);

        upgradeCostText.SetText(string.Format("Cost: {0:n0}", instanceUpgradeInfo.GetCurrentUpgradeCost()));

        buyButton.interactable = scoreLogic.CanAfford(instanceUpgradeInfo.GetCurrentUpgradeCost());
    }

    private void SetMaxLevelText()
    {
        upgradeNameText.SetText("");
        upgradeMagnitudeText.SetText("Max Level!");
        upgradeCostText.SetText("");
    }

    private void SetMagnitudeText(float magnitude, UpgradePanelDisplaySignInfo upgradePanelDisplaySignInfo, string unit)
    {
        switch (upgradePanelDisplaySignInfo)
        {
            case UpgradePanelDisplaySignInfo.ABSOLUTE_10_SCALING: upgradeMagnitudeText.SetText(string.Format("+{0:n0} {1}", magnitude * 10f  * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1), unit)); break;
            case UpgradePanelDisplaySignInfo.ABSOLUTE_SCALING: upgradeMagnitudeText.SetText(string.Format("+{0:n0} {1}", magnitude  * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1), unit)); break;
            case UpgradePanelDisplaySignInfo.ABSOLUTE_NEGATIVE_SCALING: upgradeMagnitudeText.SetText(string.Format("-{0:n0} {1}", magnitude  * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1), unit)); break;
            case UpgradePanelDisplaySignInfo.PERCENT: upgradeMagnitudeText.SetText(string.Format("+{0:n0}%", magnitude * 100f)); break;
            case UpgradePanelDisplaySignInfo.ONE_MINUS_PERCENT: upgradeMagnitudeText.SetText(string.Format("-{0:n0}%", (1f - magnitude) * 100f)); break;
            default: upgradeMagnitudeText.SetText(string.Format("+{0:n0}{1}", magnitude, unit)); break;
        }
        
    }
}
