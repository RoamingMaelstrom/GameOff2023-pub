using SOEvents;
using UnityEngine;

public class ObstacleDespawner : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent despawnEvent;
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.TryGetComponent(out AdditionalObjectInfo objectInfo)) despawnEvent.Invoke(other.gameObject);
    }
}
