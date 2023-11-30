using UnityEngine;
using SOEvents;
using UnityEngine.UI;

public class VolumeSliderLogic : MonoBehaviour
{
    [SerializeField] FloatSOEvent changeVolumeEvent;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Image volumeIcon;

    [Header("Sprites")]
    [SerializeField] Sprite normalVolumeSprite;
    [SerializeField] Sprite mutedVolumeSprite;

    [SerializeField] bool isMuted = false;

    private void Start() 
    {
        if (isMuted) volumeIcon.sprite = mutedVolumeSprite;
        else volumeIcon.sprite = normalVolumeSprite;
    }

    public void OnValueChange()
    {
        changeVolumeEvent.Invoke(volumeSlider.value);

        if (isMuted && volumeSlider.value > 0)
        {
            volumeIcon.sprite = normalVolumeSprite;
            isMuted = false;
        }

        if (volumeSlider.value <= 0 && mutedVolumeSprite && !isMuted) 
        {
            isMuted = true;
            volumeIcon.sprite = mutedVolumeSprite;
            return;
        }
    }
}
