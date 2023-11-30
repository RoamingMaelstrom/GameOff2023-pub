using SOEvents;
using UnityEngine;

public class DroneDeathHandler : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent onDroneKilledEvent;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] DropCreator dropCreator;

    private void Awake() 
    {
        onDroneKilledEvent.AddListener(HandleDroneDeath);
    }

    private void HandleDroneDeath(GameObject droneObject, float droneChainID)
    {
        BoidChain chain = boidContainer.GetBoidChainByChainID((int)droneChainID);
        Boid boid = chain.GetBoid();
        DronePurchaseInfo dronePurchaseInfo = chain.GetDronePurchaseInfo();

        if (ScaleManager.PlayerScaleGlobal <= boid.scale + 1) GliderSFX.Play.RandomAtPoint(droneObject.transform.position, "drone_death1", "drone_death2", "drone_death3");
        boid.RemoveBoid(droneObject);
        dropCreator.CreateDrops(droneObject, dronePurchaseInfo.droneSellValue, boid.scale);
    }
}
