using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;

public class ScaleManager : MonoBehaviour
{
    [SerializeField] SOEvent startScaleEvent;
    [SerializeField] SOEvent startScaleDelayedEvent;
    [SerializeField] SOEvent victoryEvent;
    [SerializeField] ScoreLogic scoreLogic;
    [field: SerializeField] public int PlayerScale {get; private set;} = 1;
    public static int PlayerScaleGlobal {get; private set;} = 1;
    [field: SerializeField] public int ScaleSizeFactor {get; private set;} = 4;
    public static int ScaleSizeFactorGlobal {get; private set;} = 4;
    [field: SerializeField] public int TotalScalingDuration {get; private set;} = 4;
    public static int TotalScalingDurationGlobal {get; private set;} = 4;

    [SerializeField] float delayedEventTime = 2f;
    [SerializeField] List<float> matterNeededToScale = new();

    private void Awake() 
    {
        PlayerScaleGlobal = PlayerScale;
        ScaleSizeFactorGlobal = ScaleSizeFactor;
        TotalScalingDurationGlobal = TotalScalingDuration;
    }

    public void TryIncrementScale()
    {
        if (!CanScale()) return;
        PlayerScale ++;
        PlayerScaleGlobal = PlayerScale;
        GliderSFX.Play.RandomStandard("scale_up1", "scale_up2");
        GliderSFX.Play.RandomStandard("scale_up3", "scale_up4");
        startScaleEvent.Invoke();
        if (PlayerScale == 5) victoryEvent.Invoke();
        StartCoroutine(RunDelayedEvent(delayedEventTime));
    }

    public bool CanScale()
    {
        return scoreLogic.GetCurrentMatter() >= matterNeededToScale[PlayerScale - 1];
    }

    private IEnumerator RunDelayedEvent(float delay)
    {
        yield return new WaitForSeconds(delay);
        startScaleDelayedEvent.Invoke();
    }

    public float GetMatterNeededToScale() => matterNeededToScale[PlayerScale - 1];
}
