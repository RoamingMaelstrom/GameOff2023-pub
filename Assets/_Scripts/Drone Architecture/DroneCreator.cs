using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class DroneCreator : UpgradeApplier
{
    [SerializeField] IntSOEvent sellDroneEvent;
    [SerializeField] ObjectPoolMain objectPool;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] BoidContainer boidsContainer;
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] string[] purchaseSfx;

    public override void ApplyUpgrade(UpgradeInfo arg0, int droneID)
    {
        BoidChain chain = boidContainer.GetBoidChainContaining(droneID);
        if (chain.chainID == -1) chain = boidContainer.GetBoidChainContaining(droneID - 5000);
        foreach (var droneInfo in chain.dronePurchaseInfoList)
        {
            if (droneID == droneInfo.dronePoolID) AddDrone(chain.GetBoid().droneID);
            if (droneID - 5000 == droneInfo.dronePoolID) AddDrones(chain.GetBoid().droneID, droneInfo);
        }
    }

    public void AddDrone(int droneID)
    {
        Boid boid = boidsContainer.GetBoid(droneID);
        GameObject drone = objectPool.GetObject(droneID);
        drone.GetComponent<Health>().FullHeal();
        drone.GetComponent<DamageDealer>().ManuallyRestartDotDamageCoroutine();
        drone.transform.position = playerBody.transform.position + (Vector3)Random.insideUnitCircle;
        boid.AddBoid(drone.GetComponent<Rigidbody2D>());

        GliderSFX.Play.RandomStandard(purchaseSfx);
    }

    private void AddDrones(int droneID, DronePurchaseInfo purchaseInfo)
    {
        for (int i = 0; i < purchaseInfo.bulkBuySize; i++)
        {
            AddDrone(droneID);
        }
    }

    public override void RunOnAwake()
    {
        sellDroneEvent.AddListener(TrySellDrones);
    }

    private void TrySellDrones(int droneID)
    {
        bool isBulk = droneID - 30000 > 5000;
        BoidChain chain = boidsContainer.GetBoidChainContaining(droneID);
        if (chain.chainID == -1) return; 

        Boid boid = chain.GetBoid();
        int sellValue = chain.GetDronePurchaseInfo().droneSellValue;

        if (!isBulk)
        {
            if (boid.numberOfDrones == 0) return;
            boid.RemoveBoid(boid.droneList.Count - 1);
            scoreLogic.AddMatterNotScore(sellValue);
            return;
        }

        int bulkSellSize = chain.dronePurchaseInfoList[0].bulkBuySize;
        if (boid.numberOfDrones < bulkSellSize) return;
        for (int i = 0; i < bulkSellSize; i++) boid.RemoveBoid(boid.droneList.Count - 1);
        scoreLogic.AddMatterNotScore(bulkSellSize * sellValue);
        return;
        
    }
}
