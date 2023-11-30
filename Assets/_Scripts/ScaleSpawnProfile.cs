using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScaleSpawnProfile", menuName = "ScaleSpawnProfileObject", order = 6)]
public class ScaleSpawnProfile : ScriptableObject
{

    [field: SerializeField] public string LayerName {get; private set;}
    [field: SerializeField] public int Scale {get; private set;}
    [field: SerializeField] public int DensityPerChunk {get; private set;}
    [field: SerializeField] public List<SpawnProbabilityGroup> SpawnProbabilityGroups {get; private set;} = new();

    private List<float> spawnGroupTotals = new();

    public static int GetObjectDensityValue(AdditionalObjectInfo additionalObjectInfo) => additionalObjectInfo.scale * additionalObjectInfo.scale;

    public List<int> GenerateListOfSpawnIDs(int length)
    {
        List<float> groupTotals = GetSpawnGroupTotals();
        List<int> totalEachGroup = GetTotalSpawnsEachGroup(groupTotals, length);
        List<int> output = new();

        for (int i = 0; i < totalEachGroup.Count; i++)
        {
            int poolIDsCount = SpawnProbabilityGroups[i].ObjectPoolIds.Count;
            for (int j = 0; j < totalEachGroup[i]; j++)
            {
                output.Add(SpawnProbabilityGroups[i].ObjectPoolIds[Random.Range(0, poolIDsCount)]);
            }
        }
        return output;
    }

    public List<int> GetAllValidPoolIDsInRange(int min, int max)
    {
        List<int> output = new();
        foreach (var group in SpawnProbabilityGroups)
        {
            foreach (int poolID in group.ObjectPoolIds) 
            {
                if (poolID < min || poolID >= max) continue;
                if (!output.Contains(poolID)) output.Add(poolID);
            }
        }

        return output;
    }

    private List<int> GetTotalSpawnsEachGroup(List<float> groupTotals, int totalSpawns)
    {
        float sumGroupTotals = groupTotals.Sum();

        List<float> rawTotalSpawns = new();
        foreach (float group in groupTotals) rawTotalSpawns.Add(group * totalSpawns / sumGroupTotals);

        List<int> output = new();
        bool addOne = false;
        foreach (float rawTotal in rawTotalSpawns) 
        {
            output.Add((int)rawTotal + (addOne ? 1 : 0));
            addOne = !addOne;
        }

        return output;
    }

    private List<float> GetSpawnGroupTotals()
    {
        if (spawnGroupTotals.Count == SpawnProbabilityGroups.Count) return spawnGroupTotals;

        List<float> output = new();

        foreach (var group in SpawnProbabilityGroups)
        {
            output.Add(group.SpawnProbabilityWeight * group.ObjectPoolIds.Count);
        }

        spawnGroupTotals = output;
        return output;
    }
}

[System.Serializable]
public class SpawnProbabilityGroup
{
    [field: SerializeField] public string GroupName {get; private set;}
    [field: SerializeField] public float SpawnProbabilityWeight {get; private set;}
    [field: SerializeField] public List<int> ObjectPoolIds {get; private set;} = new();
}