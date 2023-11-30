using System.Collections;
using UnityEngine;
using SOEvents;
using UnityEngine.UI;

public class VictoryLogic : MonoBehaviour
{
    [SerializeField] SaveObject musicVolumeSaved;
    [SerializeField] SOEvent victoryEvent;
    [SerializeField] SOEvent startScaleDelayedEvent;
    [SerializeField] Image whiteBackdrop;
    [SerializeField] GameObject victoryPanelContent;

    bool victory;

    private void Awake() 
    {
        victoryEvent.AddListener(StartVictory);
        startScaleDelayedEvent.AddListener(OpenVictoryPanel);
    }

    private void OpenVictoryPanel()
    {
        if (!victory) return;
        victoryPanelContent.SetActive(true);
    }

    private void StartVictory()
    {
        victory = true;
        StartCoroutine(FadeToWhite(ScaleManager.TotalScalingDurationGlobal));
        GliderMusic.ChangeMusic.VolumeFaded(musicVolumeSaved.GetValueFloat() / 2f, 1f);
    }

    private IEnumerator FadeToWhite(float scalingAnimationDuration)
    {
        float piDeflator = Mathf.PI * 0.5f / scalingAnimationDuration;

        float timer = 0;

        whiteBackdrop.raycastTarget = true;

        while (timer < scalingAnimationDuration)
        {
            float sinValue = Mathf.Sin(piDeflator * timer);
            whiteBackdrop.color = new Color(0.95f, 0.95f, 0.95f, sinValue);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        whiteBackdrop.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Time.timeScale = 0;
    }
}
