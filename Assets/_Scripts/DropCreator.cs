using System.Collections.Generic;
using System.Linq;
using SOEvents;
using UnityEngine;

public class DropCreator : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent despawnOnKilledEvent;
    [SerializeField] ObjectPoolMain objectPoolMain;
    [SerializeField] float baseDropSize;
    [SerializeField] List<int> dropSizes = new();
    [SerializeField] int dropObjectID;

    // Included as wonky fix to bug where Drops are sometimes continuously spawned
    [SerializeField] GameObject[] recentlyRequested;
    int posInArray = 0;

    private void Awake() 
    {
        despawnOnKilledEvent.AddListener(CreateObstacleDrops);
        dropSizes.Add(1000000000);
        recentlyRequested = new GameObject[16];
    }

    private void CreateObstacleDrops(GameObject droppingObject, float arg1)
    {
        bool createDrops = true;
        if (recentlyRequested.Contains(droppingObject)) createDrops = false;

        recentlyRequested[posInArray] = droppingObject;
        posInArray ++;
        posInArray %= 16;


        if (!createDrops) return;

        AdditionalObjectInfo additionalObjectInfo = droppingObject.GetComponent<AdditionalObjectInfo>();
        float totalMatterToDrop = additionalObjectInfo.GetMatterValue();
        CreateDrops(droppingObject, (int)totalMatterToDrop, additionalObjectInfo.scale);
    }

    public void CreateDrops(GameObject droppingObject, int totalValue, int scale)
    {
        float remaining = totalValue;
        int counter = 0;

        while (remaining > totalValue / 100 && counter < 25)
        {
            counter ++;
            for (int i = 0; i < dropSizes.Count; i++)
            {
                if (dropSizes[i + 1] > remaining)
                {
                    CreateDrop(droppingObject, dropSizes[i], i, scale);
                    remaining -= dropSizes[i];
                    break;
                }
            }
        } 
    }

    private void CreateDrop(GameObject droppingObject, int value, int sizeNum, int scale)
    {
        GameObject newDrop = objectPoolMain.GetObject(dropObjectID);
        newDrop.GetComponent<DropInfo>().dropValue = value;
        newDrop.transform.position = droppingObject.transform.position;
        newDrop.transform.localScale = Vector3.one * baseDropSize * (1 + (sizeNum / 2f)) * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal * 0.9f, scale);
        newDrop.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * scale;
    }
}
