using SOEvents;
using UnityEngine;

public class DropPickupLogic : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent pickupDropEvent;
    [SerializeField] string[] dropPickupSfx;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        GliderSFX.Play.RandomStandard(dropPickupSfx);
        pickupDropEvent.Invoke(other.gameObject, other.GetComponent<DropInfo>().dropValue);
    }
}
