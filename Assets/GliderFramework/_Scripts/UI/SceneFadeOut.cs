using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeOut : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] int loadSceneIndexOnEnd = 1;

    private void Awake() 
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void FadeInAndLoadSceneIfSet()
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float fadeDuration)
    {
        float rateOfFade = 1 / fadeDuration;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += rateOfFade * 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if (loadSceneIndexOnEnd >= 0) SceneManager.LoadScene(loadSceneIndexOnEnd);
    }
}
