using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidContainer : MonoBehaviour
{
    [SerializeField] public List<BoidChain> boidChains = new();


    private void Start() 
    {
        foreach (var boidChain in boidChains)
        {
            foreach (var boid in boidChain.boidList) boid.pointMarkGraphic.color = boidChain.boidColour;
        }
    }

    public Boid GetBoid(int droneID)
    {
        foreach (var chain in boidChains)
        {
            if (chain.Contains(droneID)) return chain.GetBoid();
        }
        return null;
    }

    public BoidChain GetBoidChainByChainID(int chainID)
    {
        return boidChains.Find(o => o.chainID == chainID);
    }

    public BoidChain GetBoidChainContaining(int droneID)
    {
        foreach (var chain in boidChains)
        {
            if (chain.Contains(droneID)) return chain;
        }
        BoidChain dudOutput = new BoidChain
        {
            chainID = -1
        };
        return dudOutput;
    }
}

[System.Serializable]
public class BoidChain
{
    public int chainID;
    public string droneDisplayName;
    public List<Boid> boidList;
    public List<DronePurchaseInfo> dronePurchaseInfoList;
    public List<int> upgradeCostList;
    public int level;
    [TextArea(2, 4)] public string droneDescription;
    public Color boidColour;

    public bool Contains(int droneID)
    {
        droneID -= droneID - 30000 > 5000 ? 5000 : 0;
        int index = boidList.FindIndex(o => o.droneID == droneID);
        return index != -1;
    }

    public Boid GetBoid()
    {
        switch (level)
        {
            case 1: return boidList[0];
            case 2: return boidList[1];
            case 3: return boidList[2];
            default: return null;
        }
    }

    public DronePurchaseInfo GetDronePurchaseInfo()
    {
        switch (level)
        {
            case 1: return dronePurchaseInfoList[0];
            case 2: return dronePurchaseInfoList[1];
            case 3: return dronePurchaseInfoList[2];
            default: return null;
        }
    }

    public bool IsMaxLevel() => level >= 3;

    public List<Boid> GetAllBoids() => boidList;

}
