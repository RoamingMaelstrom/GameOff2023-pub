using System;
using System.Collections;
using SOEvents;
using UnityEngine;
using UnityEngine.UI;

public class ClampedScrollingBackground : MonoBehaviour
{
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] float parallaxStrength;

    [SerializeField] Image image;
    RectTransform rectTransform;
    [SerializeField] bool useScalingAnimation = true;


    private void Awake() 
    {
        startScalingEvent.AddListener(StartAnimationRoutine);
    }

    private void StartAnimationRoutine()
    {
        if (!useScalingAnimation) return;
        StartCoroutine(ScalingAnimation(ScaleManager.ScaleSizeFactorGlobal));
    }

    // Todo: Might need to replace with sine wave animation (see cameraZoomOutLogic).
    // Alternatively, could just have Camera and Background expand at different rates
    private IEnumerator ScalingAnimation(float scalingAnimationDuration)
    {
        float piDeflator = Mathf.PI * 0.5f / scalingAnimationDuration;

        float timer = 0;
        float startScale = image.rectTransform.localScale.x;
        float endScale = startScale * ScaleManager.ScaleSizeFactorGlobal;

        float startParallax = parallaxStrength;
        float endParallax = parallaxStrength + ((1f - parallaxStrength) * 0.2f);

        while (timer < scalingAnimationDuration)
        {
            float sinValue = Mathf.Sin(piDeflator * timer);
            image.rectTransform.localScale = Vector3.one * (startScale +  (sinValue * (endScale - startScale)));
            parallaxStrength = startParallax + (sinValue * (endParallax - startParallax));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        image.rectTransform.localScale = Vector3.one * endScale;
        parallaxStrength = endParallax;
    }

    private void Start() 
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void LateUpdate() 
    {
        Vector3 distance = Camera.main.transform.position * parallaxStrength;
        rectTransform.anchoredPosition = distance * parallaxStrength;

        ClampImage();
    }

    private void ClampImage()
    {
        Vector2 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));

        Vector2 imageSize = image.rectTransform.rect.size * image.rectTransform.localScale;
        Vector2 imagePos = image.rectTransform.anchoredPosition;

        image.rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(imagePos.x, screenTopRight.x - (imageSize.x / 2f), screenBottomLeft.x + (imageSize.x / 2f)), Mathf.Clamp(imagePos.y, screenTopRight.y - (imageSize.y / 2f), screenBottomLeft.y + (imageSize.y / 2f)));
    }
}
