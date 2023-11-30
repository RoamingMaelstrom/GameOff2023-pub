using UnityEngine;

public class DropAttractor : MonoBehaviour
{
    public float maxAttractionAcceleration = 1f;
    [SerializeField] CircleCollider2D attractorCollider;
    [SerializeField] float attractorSize;

    private void FixedUpdate() 
    {
        attractorSize = attractorCollider.radius * transform.localScale.x;
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        Rigidbody2D otherBody = other.attachedRigidbody;
        float percentFromCentre = (other.transform.position - transform.position).magnitude / attractorSize;
        float forceSize = maxAttractionAcceleration * otherBody.mass * percentFromCentre * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal * 0.75f, ScaleManager.PlayerScaleGlobal);
        otherBody.AddForce(forceSize * (transform.position - other.transform.position).normalized);
    }
}
