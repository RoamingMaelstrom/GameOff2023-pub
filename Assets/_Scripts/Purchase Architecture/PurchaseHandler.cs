using UnityEngine;
using SOEvents;
using System.Collections.Generic;
using System;

public class PurchaseHandler : MonoBehaviour
{
    [SerializeField] IntSOEvent requestPurchaseEvent;
    [SerializeField] Int2SOEvent couldNotPurchaseEvent;
    [SerializeField] Int2SOEvent makePurchaseEvent;
    [SerializeField] IntSOEvent purchaseCompleteEvent;
    [SerializeField] InstanceUpgradeInfoSOEvent getUpgradeInfoEvent;

    [SerializeField] ScoreLogic scoreLogic;

    [SerializeField] List<UpgradeInfo> upgradeInfoObjects = new();
    [SerializeField] List<InstanceUpgradeInfo> instanceUpgradeInfos = new();
    [SerializeField] BoidContainer boidContainer;
    // Should be set to between 30000 - 40000 to work properly.
    [SerializeField] Vector2 droneIdRange;

    private void Awake() 
    {
        CreateInstanceUpgradeInfoStructs();
        requestPurchaseEvent.AddListener(ProcessPurchaseRequest);
        getUpgradeInfoEvent.AddListener(FillInstanceUpgradeInfoStruct);
    }

    private void FillInstanceUpgradeInfoStruct(InstanceUpgradeInfo toFill)
    {
        int index = instanceUpgradeInfos.FindIndex(o => o.upgradeID == toFill.upgradeID);
        if (index == -1)
        {
            Debug.Log(string.Format("No InstanceUpgradeInfo struct with upgradeID {0} found.", toFill.upgradeID));
            return;
        }


        toFill.upgradeInfo = instanceUpgradeInfos[index].upgradeInfo;
        toFill.upgradeLevel = instanceUpgradeInfos[index].upgradeLevel;
    }

    private void CreateInstanceUpgradeInfoStructs()
    {
        foreach (var infoObject in upgradeInfoObjects)
        {
            instanceUpgradeInfos.Add(new InstanceUpgradeInfo(infoObject));
        }
    }

    private void ProcessPurchaseRequest(int purchaseID)
    {
        if (purchaseID >= droneIdRange.x && purchaseID < droneIdRange.y)
        {
            ProcessDronePurchase(purchaseID);
            return;
        }
        int index = instanceUpgradeInfos.FindIndex(o => o.upgradeID == purchaseID);
        if (index == -1)
        {   
            couldNotPurchaseEvent.Invoke(purchaseID, (int)PurchaseOutcome.PURCHASE_ID_NOT_FOUND);
            return;
        }

        InstanceUpgradeInfo instanceUpgradeInfo = instanceUpgradeInfos[index];

        PurchaseOutcome purchaseOutcome = PurchaseOutcome._PURCHASE_OK;

        if (!scoreLogic.CanAfford(instanceUpgradeInfo.GetCurrentUpgradeCost())) purchaseOutcome += (int)PurchaseOutcome.PURCHASE_CANNOT_AFFORD;
        if (instanceUpgradeInfo.IsMaxLevel())  purchaseOutcome += (int)PurchaseOutcome.PURCHASE_MAX_LEVEL;

        Debug.Log(purchaseOutcome);

        if (purchaseOutcome == 0) 
        {
            makePurchaseEvent.Invoke(purchaseID, instanceUpgradeInfo.upgradeLevel);

            GliderSFX.Play.Standard("purchase");

            scoreLogic.SubtractMatter(instanceUpgradeInfo.GetCurrentUpgradeCost());
            instanceUpgradeInfo.upgradeLevel ++;

            purchaseCompleteEvent.Invoke(purchaseID);
            return;
        }

        couldNotPurchaseEvent.Invoke(purchaseID, (int)purchaseOutcome);
    }

    private void ProcessDronePurchase(int droneID)
    {
        BoidChain chain = boidContainer.GetBoidChainContaining(droneID);
        int index = chain.dronePurchaseInfoList.FindIndex(o => o.dronePoolID == (droneID - 30000 >= 5000 ? droneID - 5000 : droneID));
        if (index == -1)
        {   
            couldNotPurchaseEvent.Invoke(droneID, (int)PurchaseOutcome.PURCHASE_ID_NOT_FOUND);
            return;
        }

        PurchaseOutcome purchaseOutcome = PurchaseOutcome._PURCHASE_OK;
        Boid currentBoid = chain.GetBoid();
        DronePurchaseInfo dronePurchaseInfo = chain.dronePurchaseInfoList[index];

        bool bulkBuy = droneID - 30000 > 5000;
        int cost = bulkBuy ? dronePurchaseInfo.GetBulkCost(currentBoid.numberOfDrones) : dronePurchaseInfo.GetCost(currentBoid.numberOfDrones);

        if (!scoreLogic.CanAfford(cost)) purchaseOutcome += (int)PurchaseOutcome.PURCHASE_CANNOT_AFFORD;

        if (purchaseOutcome == 0) 
        {
            makePurchaseEvent.Invoke(droneID, bulkBuy ? dronePurchaseInfo.bulkBuySize : 1);
            scoreLogic.SubtractMatter(cost);

            purchaseCompleteEvent.Invoke(droneID);
            return;
        }

        couldNotPurchaseEvent.Invoke(droneID, (int)purchaseOutcome);
    }
}


[Flags]
public enum PurchaseOutcome
{
    _PURCHASE_OK = 0,
    PURCHASE_CANNOT_AFFORD = 1,
    PURCHASE_MAX_LEVEL = 2,
    PURCHASE_ID_NOT_FOUND = 4,
    DRONE_CAP_REACHED = 8
}
