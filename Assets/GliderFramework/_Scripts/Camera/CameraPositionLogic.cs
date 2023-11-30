using UnityEngine;

public class CameraPositionLogic : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;

    [SerializeField] float velocityCameraPositionMultiplier = 0.5f;
    [SerializeField] float zeroLerpDistance = 50f;

    Vector3 tMinus1Pos = new Vector3();
    Vector3 tMinus2Pos = new Vector3();
    Vector3 currentPos = new Vector3();

    float tMinus1 = 0.5f;

    Transform playerTransform;
    GameObject target;
    
    private void Start() 
    {
        playerTransform = playerBody.transform;
    }

    private void LateUpdate() 
    {
        tMinus2Pos = tMinus1Pos;
        tMinus1Pos = currentPos;
        Vector3 centrePosition = playerTransform.position;

        currentPos = centrePosition + ((Vector3)playerBody.velocity * velocityCameraPositionMultiplier);
        currentPos = Vector3.Lerp(currentPos, tMinus1Pos, 0.5f);
        currentPos = Vector3.Lerp(currentPos, tMinus2Pos, 0.25f);

        transform.position = currentPos;
        tMinus2Pos = tMinus1Pos;
        tMinus1Pos = currentPos;
    }
    
}
