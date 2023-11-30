using UnityEngine;

[CreateAssetMenu(fileName = "DronePurchaseInfoSO", menuName = "DronePurchaseInfoSO", order = 6)]
public class DronePurchaseInfo : ScriptableObject
{
    [field: SerializeField] public int dronePoolID {get; private set;}
    public string droneName;
    public int cost;
    public int costIncreasePerDrone;
    public int bulkBuySize;
    public int droneSellValue;

    // Todo: Remove these (seem to be unused)
    [Header("Other Stats")]
    public int scale;
    public int numberCap;

    public int GetCost(int droneCount) => cost + (costIncreasePerDrone * droneCount);
    public int GetBulkCost(int droneCount) => (cost + (costIncreasePerDrone * droneCount)) * bulkBuySize;
}
