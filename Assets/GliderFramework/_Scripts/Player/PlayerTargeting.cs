using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;

public class PlayerTargeting : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent playerStartTargetingEvent;
    [SerializeField] SOEvent playerStopTargetingEvent;

    [SerializeField] float targetSearchRadius = 2f;
    
    [SerializeField] LayerMask validObjectsMask;

    public GameObject target;
    public bool isTargeting {get; private set;} = false;

    Collider2D[] potentialTargets;



    private void FixedUpdate() => CheckIfTargetDeactivated();



    void OnRMB(InputValue value)
    {
        if (value.Get<float>() != 0) return;
        UpdateTarget(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }



    private void CheckIfTargetDeactivated()
    {
        if (!target) return;
        if (target.activeInHierarchy) return;

        StopTargeting();
    }

    private GameObject GetTargetNearestToCentre(Collider2D[] potentialTargets, Vector3 centre)
    {
        if (potentialTargets.Length == 1) return potentialTargets[0].gameObject;

        Collider2D closestCollider = potentialTargets[0];
        float smallestDistanceSqr = (centre - closestCollider.transform.position).sqrMagnitude;
        float testDistanceSqr;

        for (int i = 1; i < potentialTargets.Length; i++)
        {
            testDistanceSqr = (centre - potentialTargets[i].transform.position).sqrMagnitude;
            if (testDistanceSqr < smallestDistanceSqr) 
            {   
                closestCollider = potentialTargets[i];
                smallestDistanceSqr = testDistanceSqr;
            }
        }

        return closestCollider.gameObject;
    }

    private void StartTargeting(GameObject tempTarget)
    {
        target = tempTarget;
        isTargeting = true;
        playerStartTargetingEvent.Invoke(target);
    }

    private void StopTargeting()
    {
        target = null;
        isTargeting = false;
        playerStopTargetingEvent.Invoke();
    }

    private void UpdateTarget(Vector2 mouseWorldPos)
    {
        potentialTargets = Physics2D.OverlapCircleAll(mouseWorldPos, targetSearchRadius, validObjectsMask);

        if (potentialTargets.Length == 0) 
        {
            StopTargeting();
            return;
        }

        GameObject potentialTarget = GetTargetNearestToCentre(potentialTargets, mouseWorldPos);

        if (!target) StartTargeting(potentialTarget);
        else if (potentialTarget.GetInstanceID() == target.GetInstanceID()) StopTargeting();
        else StartTargeting(potentialTarget);
    }
}
