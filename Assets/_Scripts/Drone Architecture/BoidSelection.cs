using System;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class BoidSelection : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent selectedBoidEvent;
    [SerializeField] IntSOEvent mouseUpEvent;
    [SerializeField] BoidController boidController;
    [SerializeField] LayerMask followSelectionMask;
    [SerializeField] Rigidbody2D playerBody;

    [SerializeField] float doubleClickTimerThreshold = 0.25f;
    private float doubleClickTimer;

    private void Awake() 
    {   
        mouseUpEvent.AddListener(HandleMouseClickUp);
    }

    private void FixedUpdate() 
    {
        doubleClickTimer -= Time.fixedDeltaTime;
    }

    private void HandleMouseClickUp(int mouseClickUp)
    {
        if (MouseInfo.mouseOverUIGlobal) return;
        Vector2 mousePosWorld = MouseInfo.mousePosWorldGlobal;
        if (mouseClickUp == 0) TrySelectBoid(mousePosWorld);
        if (mouseClickUp == 2) TryMoveBoidCentre(mousePosWorld);
    }

    private void TrySelectBoid(Vector2 mousePosWorld)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePosWorld, 0.25f * ScaleManager.PlayerScaleGlobal * ScaleManager.PlayerScaleGlobal);
        foreach (var collider in hits)
        {
            if (collider.TryGetComponent(out BoidObstacleDetector boidObstacleDetector))
            {   
                Boid boid = boidObstacleDetector.transform.parent.GetComponent<Boid>();
                if (boid == boidController.boidControlling)
                {
                    boidController.SetBoidControlling(null);
                    selectedBoidEvent.Invoke(null);
                    return;
                }
                if (boid.numberOfDrones == 0) continue;
                boidController.SetBoidControlling(boid);
                doubleClickTimer = 0;
                selectedBoidEvent.Invoke(boid.gameObject);
                return;
            }
        }

        boidController.SetBoidControlling(null);
        selectedBoidEvent.Invoke(null);
    }

    private void TryMoveBoidCentre(Vector2 mousePosWorld)
    {
        if (boidController.boidControlling == null) return;
        if (boidController.followSelectedObject) boidController.selectedObject = SetNewSelectedObject(mousePosWorld);
        boidController.centrePos = mousePosWorld;
        GliderSFX.Play.Standard("direct_drone");
    }

    private GameObject SetNewSelectedObject(Vector2 mousePosWorld)
    {
        float searchRadius = 0.25f * ScaleManager.PlayerScaleGlobal * ScaleManager.PlayerScaleGlobal;
        if ((mousePosWorld - (Vector2)playerBody.transform.position).magnitude < searchRadius) return playerBody.gameObject;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePosWorld, searchRadius, followSelectionMask);

        GameObject nearestObject = null;
        float nearestDistanceSqr = Mathf.Infinity;
        float testingDistanceSqr;

        foreach (var collider in hits)
        {
            if (collider.TryGetComponent(out AdditionalObjectInfo additionalObjectInfo))
            {   
                testingDistanceSqr = (mousePosWorld - (Vector2)collider.transform.position).sqrMagnitude;
                if (testingDistanceSqr < nearestDistanceSqr)
                {   
                    nearestObject = collider.gameObject;
                    nearestDistanceSqr = testingDistanceSqr;
                }
            }
        }
        return nearestObject;
    }
}
