using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float fadeDelay = 0.75f;
    [SerializeField] float fadeDuration = 0.5f;

    private void OnEnable() 
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        StartCoroutine(FadeAnimation());
    }

    private IEnumerator FadeAnimation()
    {
        yield return new WaitForSeconds(fadeDelay);

        float piDeflator = Mathf.PI * 0.5f / fadeDuration;
        float timer = 0;

        while (timer < fadeDuration)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Cos(timer * piDeflator));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
}
