using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeOpenDuration = 0.2f;
    [SerializeField] float fadeCloseDuration = 0.2f;
    [SerializeField] float targetScale = 1f;

    Coroutine animationCoroutine;


    private void OnEnable() 
    {
        Open();
    }

    public void Toggle()
    {
        if (canvasGroup.alpha == 1) Open();
        else Close();
    }


    public void Open()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(Animation(fadeOpenDuration, true));
    }

    private IEnumerator Animation(float fadeDuration, bool openDirection = true)
    {
        float rateOfFade = Time.fixedDeltaTime / fadeDuration;
        float rateOfSizeChange = Time.fixedDeltaTime * targetScale / fadeDuration;
        rateOfFade *= openDirection ? 1 : -1;
        rateOfSizeChange *= openDirection ? 1 : -1;

        if (!openDirection) canvasGroup.interactable = false;

        while (canvasGroup.alpha * (openDirection ? 1: -1) < (openDirection ? 1 : 0))
        {
            canvasGroup.alpha += rateOfFade;
            canvasGroup.transform.localScale = canvasGroup.transform.localScale + (Vector3.one * rateOfSizeChange);

            yield return new WaitForFixedUpdate();
        }

        if (openDirection) canvasGroup.interactable = true;
    }

    public void Close()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(Animation(fadeCloseDuration, false));
    }
}
