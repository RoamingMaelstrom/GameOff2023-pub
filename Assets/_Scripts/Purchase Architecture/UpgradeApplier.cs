using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public abstract class UpgradeApplier : MonoBehaviour
{
    [SerializeField] Int2SOEvent makePurchaseEvent;
    [SerializeField] List<UpgradeInfo> upgradeInfosTracking;
    [SerializeField] bool dronePurchaser = false;

    private void Awake() 
    {
        makePurchaseEvent.AddListener(ProcessPurchase);
        RunOnAwake();
    }

    private void ProcessPurchase(int upgradeID, int upgradeLevel)
    {
        foreach (var item in upgradeInfosTracking)
        {
            if (item.upgradeID == upgradeID)
            {
                ApplyUpgrade(item, upgradeLevel);
                return;
            }
        }

        if (dronePurchaser && upgradeID > 30000) ApplyUpgrade(upgradeInfosTracking[0], upgradeID);

    }

    public abstract void RunOnAwake();
    public abstract void ApplyUpgrade(UpgradeInfo upgradeInfo, int upgradeLevel);
}
