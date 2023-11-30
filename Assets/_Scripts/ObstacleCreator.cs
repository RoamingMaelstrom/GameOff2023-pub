using UnityEngine;
using System.Collections.Generic;
using SOEvents;

public class ObstacleCreator : MonoBehaviour
{
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] SOEvent delayedScalingEvent;
    [SerializeField] Vector2Vector2SOEvent populateChunkEvent;
    [SerializeField] ObjectPoolMain objectPoolMain;
    [SerializeField] LayerMask obstacleLayerMask;
    [SerializeField] LayerMask noInteractionsLayerMask;
    [SerializeField] ScaleSpawnProfile scale1SpawnProfile;
    [SerializeField] ScaleSpawnProfile scale2SpawnProfile;
    [SerializeField] ScaleSpawnProfile scale3SpawnProfile;
    [SerializeField] ScaleSpawnProfile scale4SpawnProfile;
    [SerializeField] ScaleSpawnProfile scale5SpawnProfile;
    [SerializeField] int currentScale = 1;
    [SerializeField] ScaleSpawnProfile currentScaleSpawnProfile;

    [SerializeField] List<int> objectIDsToSpawn = new();
    [SerializeField] List<int> objectIDsToSpawnBackground = new();

    [SerializeField] [Range(0f, 1f)] float backgroundSpawnProbabilityPerChunk = 0.25f;


    [SerializeField] List<AdditionalObjectInfo> backgroundPlanets4 = new();
    [SerializeField] List<int> planetIds = new();

    private void Awake() 
    {
        startScalingEvent.AddListener(RunOnScaleIncrease);
        delayedScalingEvent.AddListener(ChangeObjectLayers);
        populateChunkEvent.AddListener(PopulateArea);
        currentScaleSpawnProfile = scale1SpawnProfile;
    }

    private void ChangeObjectLayers()
    {
        Vector2 range = new Vector2(1000 + (100 * currentScale), 1100 + (100 * currentScale));
        SetLayerOfPoolObjects(currentScaleSpawnProfile.GetAllValidPoolIDsInRange((int)range.x, (int)range.y), currentScaleSpawnProfile.LayerName); 
    }

    private void RunOnScaleIncrease()
    {
        currentScale ++;
        switch (currentScale)
        {
            case 2: currentScaleSpawnProfile = scale2SpawnProfile; break;
            case 3: currentScaleSpawnProfile = scale3SpawnProfile; break;
            case 4: currentScaleSpawnProfile = scale4SpawnProfile; break;
            case 5: currentScaleSpawnProfile = scale5SpawnProfile; break;
            default: break;
        }
        
        objectIDsToSpawn = GenerateObjectIDsToSpawn();
        objectIDsToSpawnBackground = GenerateObjectIDsToSpawnBackground();  

        backgroundSpawnProbabilityPerChunk += 0.1f;
    }

    private List<int> GenerateObjectIDsToSpawnBackground()
    {
        switch (currentScale)
        {
            case 1: return scale2SpawnProfile.GenerateListOfSpawnIDs(50);
            case 2: return scale3SpawnProfile.GenerateListOfSpawnIDs(75);
            case 3: return scale4SpawnProfile.GenerateListOfSpawnIDs(20);
            case 4: return scale5SpawnProfile.GenerateListOfSpawnIDs(20);
            // Todo: Add Planets in background of scale 4
            default: return new();
        }
    }

    private List<int> GenerateObjectIDsToSpawn()
    {
        switch (currentScale)
        {
            case 1: return scale1SpawnProfile.GenerateListOfSpawnIDs(200);
            case 2: return scale2SpawnProfile.GenerateListOfSpawnIDs(300);
            case 3: return scale3SpawnProfile.GenerateListOfSpawnIDs(400);
            case 4: return scale4SpawnProfile.GenerateListOfSpawnIDs(400);
            case 5: return scale5SpawnProfile.GenerateListOfSpawnIDs(800);
            default: return new();
        }
    }

    private int GetNextObjectID(List<int> objectIDs, bool isBackground)
    {   
        if (objectIDs.Count == 0) 
        {
            if (isBackground) objectIDs = GenerateObjectIDsToSpawnBackground();
            else objectIDs = GenerateObjectIDsToSpawn();
        }

        int index = Random.Range(0, objectIDs.Count);
        int output = objectIDs[index];
        objectIDs.RemoveAt(index);
        return output;
    }

    private void SetLayerOfPoolObjects(List<int> poolIDs, string newLayer)
    {
        foreach (int poolID in poolIDs)
        {
            objectPoolMain.ChangePoolObjectsLayers(poolID, newLayer);
        }
    }

    private void PopulateForeground(Vector2 chunkCentre, Vector2 chunkSize)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(chunkCentre, chunkSize, obstacleLayerMask);
        float density = 0;
        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("no_interaction")) continue;
            if (hit.gameObject.TryGetComponent(out AdditionalObjectInfo objectInfo)) density += ScaleSpawnProfile.GetObjectDensityValue(objectInfo);
        }

        int counter = 0;

        while (counter < 100 && density < currentScaleSpawnProfile.DensityPerChunk)
        {
            counter ++;
            Vector3 spawnPos = GenerateRandomSpawnPos(chunkCentre, chunkSize / 2f);
            AdditionalObjectInfo newObjectInfo = SpawnObstacle(GetNextObjectID(objectIDsToSpawn, false), spawnPos, false);
            density += ScaleSpawnProfile.GetObjectDensityValue(newObjectInfo);
        }
    }

    private void PopulateBackground(Vector2 chunkCentre, Vector2 chunkSize)
    {
        if (currentScale >= 5) return;

        if (currentScale == 4) 
        {
            PopulateBackgroundPlanets(chunkCentre, chunkSize);
            return;
        }

        if (Random.Range(0f, 1f) > backgroundSpawnProbabilityPerChunk) return;  

        Collider2D[] overlapCheckHits = Physics2D.OverlapBoxAll(chunkCentre, chunkSize * 4f, noInteractionsLayerMask);

        foreach (var hit in overlapCheckHits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("no_interactions")) return;
        }
        
        Vector3 spawnPos = GenerateRandomSpawnPos(chunkCentre, chunkSize / 2f);
        SpawnObstacle(GetNextObjectID(objectIDsToSpawnBackground, true), spawnPos, true);
    }

    private void PopulateBackgroundPlanets(Vector2 chunkCentre, Vector2 chunkSize)
    {

        if (Random.Range(0.0f, 1.0f) > 0.08f) return;

        Vector3 spawnPos;
        int counter = 0;

        do
        {   
            counter ++;
            spawnPos = GenerateRandomSpawnPos(chunkCentre, chunkSize * 1.25f);
        }
        while(NearOtherPlanet(spawnPos) && counter < 100);

        backgroundPlanets4.Add(SpawnObstacle(planetIds[Random.Range(0, planetIds.Count)], spawnPos, true));
    }

    private bool NearOtherPlanet(Vector3 spawnPos)
    {
        foreach (var planet in backgroundPlanets4)
        {
            if ((planet.transform.position - spawnPos).magnitude < 260) return true;
        }
        return false;
    }

    private void PopulateArea(Vector2 chunkCentre, Vector2 chunkSize)
    {
        PopulateForeground(chunkCentre, chunkSize);
        PopulateBackground(chunkCentre, chunkSize);
    }

    private Vector3 GenerateRandomSpawnPos(Vector2 chunkCentre, Vector2 chunkSize)
    {
        return new Vector2(Random.Range(- chunkSize.x, chunkSize.x), Random.Range(- chunkSize.y, chunkSize.y)) + chunkCentre;
    }

    private AdditionalObjectInfo SpawnObstacle(int objectPoolID, Vector3 spawnPos, bool isBackground)
    {
        GameObject obstacle = objectPoolMain.GetObject(objectPoolID);
        obstacle.transform.position = spawnPos;
        obstacle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

        AdditionalObjectInfo obstacleInfo = obstacle.GetComponent<AdditionalObjectInfo>();
        Health obstacleHealth = obstacle.GetComponent<Health>();
        Rigidbody2D obstacleBody = obstacle.GetComponent<Rigidbody2D>();

        obstacleInfo.GenerateRandomSize();
        obstacleInfo.transform.localScale = Vector2.one * obstacleInfo.size;
        obstacleBody.velocity = Random.insideUnitCircle.normalized * obstacleInfo.GetRandomSpeed();
        obstacleBody.velocity *= isBackground ? 0.4f : 1f;
        obstacleInfo.matter = obstacleInfo.GetMatterValue();
        if (obstacleInfo.hasStartingTorque) obstacleBody.AddTorque(Random.Range(-1f, 1f) * obstacleBody.mass);

        obstacleHealth.maxHp = obstacleInfo.GetHealthValue();
        obstacleHealth.FullHeal();

        return obstacleInfo;
    }
}
