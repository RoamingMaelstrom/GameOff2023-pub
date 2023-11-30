using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOEvents;
using System;

public class BackgroundVisibilityLogic : MonoBehaviour
{
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] Image background1;
    [SerializeField] Image background2;
    [SerializeField] SpriteRenderer background3;
    [SerializeField] SpriteRenderer background4;
    [SerializeField] SpriteRenderer background5;
    [SerializeField] [Range(0f, 1f)] float targetOpacity = 0.9f;

    private void Awake() 
    {
        startScalingEvent.AddListener(changeScale);
    }

    private void changeScale()
    {
        switch (ScaleManager.PlayerScaleGlobal)
        {
            case 2: StartCoroutine(SwitchBackgroundsImages(background1, background2)); break;
            case 3: StartCoroutine(SwitchBackgroundsImageSprite(background2, background3)); break;
            case 4: StartCoroutine(SwitchBackgroundsSprites(background3, background4)); break;
            case 5: StartCoroutine(SwitchBackgroundsSprites(background4, background5)); break;
            default: break;
        }
    }

    private IEnumerator SwitchBackgroundsSprites(SpriteRenderer before, SpriteRenderer after)
    {
        float piDeflator = Mathf.PI * 0.5f / ScaleManager.TotalScalingDurationGlobal;

        float timer = 0;
        while (timer < ScaleManager.TotalScalingDurationGlobal)
        {
            before.color = new Color(1, 1, 1, Mathf.Cos(piDeflator * timer) * targetOpacity);
            after.color = new Color(1, 1, 1, Mathf.Sin(piDeflator * timer) * targetOpacity);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        before.color = new Color(1, 1, 1, 0);
        after.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator SwitchBackgroundsImageSprite(Image before, SpriteRenderer after)
    {
        float piDeflator = Mathf.PI * 0.5f / ScaleManager.TotalScalingDurationGlobal;

        float timer = 0;
        while (timer < ScaleManager.TotalScalingDurationGlobal)
        {
            before.color = new Color(1, 1, 1, Mathf.Cos(piDeflator * timer) * targetOpacity);
            after.color = new Color(1, 1, 1, Mathf.Sin(piDeflator * timer) * targetOpacity);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        before.color = new Color(1, 1, 1, 0);
        after.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator SwitchBackgroundsImages(Image before, Image after)
    {
        float piDeflator = Mathf.PI * 0.5f / ScaleManager.TotalScalingDurationGlobal;

        float timer = 0;
        while (timer < ScaleManager.TotalScalingDurationGlobal)
        {
            before.color = new Color(1, 1, 1, Mathf.Cos(piDeflator * timer) * targetOpacity);
            after.color = new Color(1, 1, 1, Mathf.Sin(piDeflator * timer) * targetOpacity);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        before.color = new Color(1, 1, 1, 0);
        after.color = new Color(1, 1, 1, 1);
    }
}
