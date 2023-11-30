using UnityEngine;
using SOEvents;

public class SellDroneButton : MonoBehaviour
{
    [SerializeField] IntSOEvent sellDroneEvent;

    [SerializeField] int droneID;
    [SerializeField] bool isBulk;

    public void TrySell()
    {
        sellDroneEvent.Invoke(isBulk ? droneID + 5000: droneID);
    }
}
