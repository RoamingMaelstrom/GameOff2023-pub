using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    // The unique ID of the pool
    public int poolID;
    public string poolName;
    public GameObject objectPrefab;
    public int maxPoolSize;
    public PoolDynamicOptions dynamicOptions = new();
    [SerializeField] int numObjectsFree;

    public int CurrentPoolSize 
    {
        get {return objectsFree.Count + objectsInUse.Count;} 
        private set {}
    }
    public Stack<GameObject> objectsFree = new Stack<GameObject>();
    public List<GameObject> objectsInUse = new List<GameObject>();

    private bool currentlyShrinking = false;

    // Maximum depth changed is 2 (original layer and all child objects).
    public void ChangeObjectsLayer(string newLayer)
    {
        int layer = LayerMask.NameToLayer(newLayer);

        foreach (GameObject item in objectsFree)
        {
            item.layer = layer;
            foreach (Transform child in item.transform) child.gameObject.layer = layer;
        }

        foreach (GameObject item in objectsInUse)
        {
            item.layer = layer;
            foreach (Transform child in item.transform) child.gameObject.layer = layer;
        }
    }


    public void HandleDynamicShrinking(int numberOfFixedUpdates)
    {
        if (!dynamicOptions.dynamicShrinkingEnabled) return;

        if (currentlyShrinking) ShrinkPool();

        if (objectsInUse.Count / (float)maxPoolSize > dynamicOptions.shrinkThresholdPercent) return;

        if (numberOfFixedUpdates % (int)(dynamicOptions.checkDelaySeconds / Time.fixedDeltaTime) == 0) 
        {
            maxPoolSize = Mathf.Max((int)(maxPoolSize * (1f - dynamicOptions.shrinkByPercent)), dynamicOptions.minimumPoolSize);
            currentlyShrinking = true;
        }
    }

    private void ShrinkPool()
    {
        int deltaSize = objectsInUse.Count + objectsFree.Count - maxPoolSize;
        if (deltaSize <= 0)
        {
            currentlyShrinking = false;
            return;
        }

        int shrinkAmount = Mathf.Clamp((int)(deltaSize * Time.fixedDeltaTime / (dynamicOptions.checkDelaySeconds * 0.25f)), 1, deltaSize);
        for (int i = 0; i < shrinkAmount; i++)
        {            
            if (objectsFree.Count == 0) break;
            Object.Destroy(objectsFree.Pop());
        }
    }


    // Used internally by Pool to Get an object from a specified Pool.
    /* Getobject process works as follows:
        - If there IS NOT an object free, return the Object that has been active for the longest.
        - If there IS an object free, pop it off the Free Object stack and return it.
        - Object will always be SetActive on retrieval.
    */
    public GameObject GetObject()
    {
        GameObject objectToGet;

        if (objectsFree.Count == 0) 
        {
            objectToGet = objectsInUse[0];
            objectsInUse.RemoveAt(0);
        }
        else objectToGet = objectsFree.Pop();

        objectToGet.SetActive(true);
        objectsInUse.Add(objectToGet);

        return objectToGet;
    }

    // Instantiates an instance of an object pools prefab, 
    // Adds an ObjectIdentifier Component to the new Object (with Pool information data), disables it, and then assigns it to the pool.objectsFree.
    // Also sets objectPoolMain as parent of new object.
    public GameObject CreatePoolObject(MonoBehaviour objectPoolMain)
    {
        GameObject newObject = Object.Instantiate(objectPrefab, objectPoolMain.transform);
        if (!newObject.TryGetComponent(out ObjectIdentifier identity)) identity = newObject.AddComponent<ObjectIdentifier>();

        identity.poolID = poolID;
        identity.poolName = poolName;
        
        newObject.gameObject.SetActive(false);
        objectsFree.Push(newObject);
        return newObject;
    }

    public void HandleDynamicExpansion(ObjectPoolMain objectPool)
    {
        if (!dynamicOptions.dynamicExpansionEnabled) return;

        if (CurrentPoolSize < maxPoolSize) DynamicPopulate(objectPool);

        if ((float) objectsInUse.Count / maxPoolSize >= dynamicOptions.expandThresholdPercent)
        {
            maxPoolSize = (int)(maxPoolSize * (1f + dynamicOptions.expandByPercent));
        }
    }

    private void DynamicPopulate(ObjectPoolMain objectPool)
    {
        int deltaSize = maxPoolSize - CurrentPoolSize;
        int numObjectsToCreate = Mathf.Clamp((int)(deltaSize * Time.fixedDeltaTime * 5f), 1, Mathf.Max(1, (int)(dynamicOptions.maxDynamicFillRatePerSecond * Time.fixedDeltaTime)));

        for (int i = 0; i < numObjectsToCreate; i++) CreatePoolObject(objectPool);
    }

    public void UpdateEditorObjectsFreeNumberDisplays()
    {
        numObjectsFree = objectsFree.Count;
    }
}



[System.Serializable]
public class PoolDynamicOptions
{
    [Tooltip("Determines whether pools will expand when the number of objects currently active reaches the threshold.")]
    [SerializeField] public bool dynamicExpansionEnabled = false;
    [SerializeField] [Range(0f, 1f)] public float expandThresholdPercent = 0.8f;
    [SerializeField] [Range(0f, 1f)] public float expandByPercent = 0.25f;
    [SerializeField] public int maxDynamicFillRatePerSecond = 50;

    [Space()]
    [Tooltip("Determines whether pools will expand when the number of objects currently active reaches the threshold")]
    [SerializeField] public bool dynamicShrinkingEnabled = false;
    [SerializeField] [Range(1f, 100f)] public float checkDelaySeconds = 30;
    [SerializeField] [Range(0f, 1f)] public float shrinkThresholdPercent = 0.25f;
    [SerializeField] [Range(0f, 1f)] public float shrinkByPercent = 0.25f;
    [SerializeField] public int minimumPoolSize = 25;
}


