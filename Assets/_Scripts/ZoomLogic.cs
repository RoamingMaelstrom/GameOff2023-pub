using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomLogic : MonoBehaviour
{
    [SerializeField] CameraZoomOutLogic cameraZoomOutLogic;
    [SerializeField] float minMultiplier = 1f;
    [SerializeField] float maxMultiplier = 2f;

    public void OnZoomControl(InputValue value)
    {
        float dir = value.Get<float>();
        if (Time.timeScale == 0) return;
        if (dir > 0) cameraZoomOutLogic.zoomMultiplier -= 0.1f;
        if (dir < 0) cameraZoomOutLogic.zoomMultiplier += 0.1f;
        
        cameraZoomOutLogic.zoomMultiplier = Mathf.Clamp(cameraZoomOutLogic.zoomMultiplier, minMultiplier, maxMultiplier);
    }

}
