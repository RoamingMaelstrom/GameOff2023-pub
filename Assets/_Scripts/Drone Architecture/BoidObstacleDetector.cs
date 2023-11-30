using UnityEngine;

public class BoidObstacleDetector : MonoBehaviour
{
    [SerializeField] Boid parentBoid;

    private void FixedUpdate() 
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent(out AdditionalObjectInfo objectInfo)) parentBoid.collidersInRange.Add(other);

    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.TryGetComponent(out AdditionalObjectInfo objectInfo)) parentBoid.collidersInRange.Remove(other);
    }
}
