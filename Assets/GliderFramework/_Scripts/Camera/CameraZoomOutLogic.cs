using UnityEngine;
using Cinemachine;
using SOEvents;
using System;
using System.Collections;

public class CameraZoomOutLogic : MonoBehaviour
{
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] Rigidbody2D trackingBody;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] float defaultCameraZoom = 15f;
    [SerializeField] float speedMultiplier = 2f;
    [Tooltip("Speed at which player has to be travelling for the zoom effect to take effect")]
    [SerializeField] float startSpeed = 10f;
    [SerializeField] [Range(1f, 2f)] public float zoomMultiplier = 1.3f;

    
    private void Awake() 
    {
        startScalingEvent.AddListener(StartScalingRoutine);
        defeatEvent.AddListener(StartZoomingAnimation);
    }

    private void StartZoomingAnimation()
    {
        StartCoroutine(ZoomingAnimation());
    }

    private void StartScalingRoutine()
    {
        if (ScaleManager.PlayerScaleGlobal < 5) StartCoroutine(ScalingAnimation());
        else StartCoroutine(ZoomingAnimation());
    }

    private IEnumerator ScalingAnimation()
    {

        float startZoom = defaultCameraZoom;
        float endZoom = defaultCameraZoom * ScaleManager.ScaleSizeFactorGlobal;
        float piDeflator = Mathf.PI * 0.5f / ScaleManager.TotalScalingDurationGlobal;

        float timer = 0;
        while (timer < ScaleManager.TotalScalingDurationGlobal)
        {
            defaultCameraZoom = startZoom + (Mathf.Sin(piDeflator * timer) * (endZoom - startZoom));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        defaultCameraZoom = endZoom;
    }

    private IEnumerator ZoomingAnimation()
    {

        float startZoom = defaultCameraZoom;
        float endZoom = defaultCameraZoom * 0.5f;
        float piDeflator = Mathf.PI * 0.5f / ScaleManager.TotalScalingDurationGlobal;

        float timer = 0;
        while (timer < ScaleManager.TotalScalingDurationGlobal)
        {
            defaultCameraZoom = startZoom + (Mathf.Sin(piDeflator * timer) * (endZoom - startZoom));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        defaultCameraZoom = endZoom;
    }

    private void Update() 
    {
        float zoomValue = defaultCameraZoom * zoomMultiplier;
        if (trackingBody.velocity.magnitude < startSpeed) vCam.m_Lens.OrthographicSize = zoomValue;
        else
        {
            float speedZoomOut = Mathf.Sqrt(trackingBody.velocity.magnitude) - Mathf.Sqrt(startSpeed);
            vCam.m_Lens.OrthographicSize = zoomValue + (speedZoomOut * speedMultiplier);
        }
    }

}
