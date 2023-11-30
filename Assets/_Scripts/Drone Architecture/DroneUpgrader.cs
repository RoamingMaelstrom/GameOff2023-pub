using UnityEngine;
using SOEvents;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class DroneUpgrader : MonoBehaviour
{
    [SerializeField] IntSOEvent upgradeDroneEvent;
    [SerializeField] IntSOEvent boidUpgradedEvent;
    [SerializeField] DroneCreator droneCreator;    
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] ScoreLogic scoreLogic;


    private void Awake() 
    {
        upgradeDroneEvent.AddListener(TryUpgradeDrone);
        boidUpgradedEvent.AddListener(PlayDroneUpgradedSfxDelayed);
    }

    private void PlayDroneUpgradedSfxDelayed(int arg0)
    {
        StartCoroutine(PlayUpgradeSfxDelayed(0.2f));
    }

    private IEnumerator PlayUpgradeSfxDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        GliderSFX.Play.RandomStandard("drone_upgraded", "drone_unlocked");
    }

    private void TryUpgradeDrone(int droneID)
    {
        BoidChain chain = boidContainer.GetBoidChainContaining(droneID);
        int upgradeCost = chain.upgradeCostList[chain.level];

        if (chain.IsMaxLevel()) return;
        if (!scoreLogic.CanAfford(upgradeCost)) return;
        if (ScaleManager.PlayerScaleGlobal < chain.boidList[0].scale + chain.level) return;

        scoreLogic.SubtractMatter(upgradeCost);

        Boid boidToUpgrade = chain.GetBoid();
        chain.level ++;
        Boid upgradedBoid = chain.GetBoid();
        ReplaceAllDrones(boidToUpgrade, upgradedBoid);
        

        GliderSFX.Play.Standard("purchase");


        boidUpgradedEvent.Invoke(chain.chainID);
    }

    private void ReplaceAllDrones(Boid boidToUpgrade, Boid upgradedBoid)
    {
        upgradedBoid.gameObject.SetActive(true);
        if (boidToUpgrade == null) return;
        int counter = 0;
        while (boidToUpgrade.numberOfDrones > 0 && counter < 50)
        {
            counter ++;
            droneCreator.AddDrone(upgradedBoid.droneID);
            boidToUpgrade.RemoveBoid(boidToUpgrade.numberOfDrones - 1);
        } 
    }
}
