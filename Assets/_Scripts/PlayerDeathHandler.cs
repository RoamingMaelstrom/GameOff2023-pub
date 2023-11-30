using SOEvents;
using UnityEngine;


public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] Health playerHealth;
    [SerializeField] Material vignetteMaterial;
    [SerializeField] float vignetteStrengthIncrease = 0.05f;
    [SerializeField] float vignetteSoftnessIncrease = 0.1f;

    [SerializeField] float startStrength = 0.1f;
    [SerializeField] float startSoftness = 0.1f;

    [SerializeField] float deathDuration = 5f;
    [SerializeField] string playerLowMatterSfx;



    float cachedStrength = 0;
    float cachedSoftness = 0;

    float currentStrength;
    float currentSoftness;

    bool defeated = false;

    [SerializeField] float timer = 0;

    [SerializeField] bool enteredNegativeMatter = false;
    [SerializeField] int negativeMatterAudioSourceId = -1;

    private void Start() 
    {
        cachedStrength = startStrength;
        cachedSoftness = startSoftness;

        vignetteMaterial.SetFloat("_VRadius", cachedStrength);
        vignetteMaterial.SetFloat("_VSoft", cachedSoftness);
    }

    private void FixedUpdate() 
    {
        if (defeated) return;
        if (playerHealth.GetCurrentHp() <= 0) 
        {
            timer += Time.fixedDeltaTime;

            if (!enteredNegativeMatter)
            {
                enteredNegativeMatter = true;
                negativeMatterAudioSourceId = GliderSFX.Play.Standard(playerLowMatterSfx);
            }
            GliderSFX.SfxAudioSourceInfo sourceInfo = GliderSFX.Get.AudioSourceInfoByIndex(negativeMatterAudioSourceId);
            if (sourceInfo != null) Mathf.Clamp(sourceInfo.source.volume += Time.fixedDeltaTime / 2f, 0f, 1f);
        }
        else 
        {
            timer -= Time.fixedDeltaTime * 5f;
            if (negativeMatterAudioSourceId != -1) 
            {
                Mathf.Clamp(GliderSFX.Get.AudioSourceInfoByIndex(negativeMatterAudioSourceId).source.volume -= Time.fixedDeltaTime, 0f, 1f);
                
                if (GliderSFX.Get.AudioSourceInfoByIndex(negativeMatterAudioSourceId).source.volume <= 0)
                {
                    GliderSFX.Get.AudioSourceInfoByIndex(negativeMatterAudioSourceId).source.Stop();
                    negativeMatterAudioSourceId = -1;
                    enteredNegativeMatter = false;
                }
            }
        }

        timer = Mathf.Clamp(timer, 0f, deathDuration);

        if (timer >= deathDuration) 
        {
            defeatEvent.Invoke();
            defeated = true;
        }

        currentStrength =  startStrength + (timer * vignetteStrengthIncrease);
        currentSoftness = startSoftness + (timer * vignetteSoftnessIncrease);

        if (currentStrength != cachedStrength)
        {
            vignetteMaterial.SetFloat("_VRadius", currentStrength);
            cachedStrength = currentStrength;
        }

        if (currentSoftness != cachedSoftness)
        {
            vignetteMaterial.SetFloat("_VSoft", currentSoftness);
            cachedSoftness = currentSoftness;
        }
    }
}
