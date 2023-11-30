using UnityEngine;

public class BoidRangedTargeting : MonoBehaviour
{
    [SerializeField] Boid parentBoid;

    private void Start() 
    {
        GetComponent<CircleCollider2D>().radius = 3 * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, parentBoid.scale - 1);

    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.TryGetComponent(out AdditionalObjectInfo objectInfo)) parentBoid.targetsForRangedDrones.Add(objectInfo);
        if (other.TryGetComponent(out EnemyAI aiModule)) parentBoid.enemiesInRange.Add(aiModule);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.TryGetComponent(out AdditionalObjectInfo objectInfo))
        {   
            int index = parentBoid.targetsForRangedDrones.FindIndex(o => o.gameObject.GetInstanceID() == other.gameObject.GetInstanceID());
            if (index != -1) parentBoid.targetsForRangedDrones.RemoveAt(index);
        }

        if (other.TryGetComponent(out EnemyAI aiModule)) parentBoid.enemiesInRange.Remove(aiModule);
    }
}
