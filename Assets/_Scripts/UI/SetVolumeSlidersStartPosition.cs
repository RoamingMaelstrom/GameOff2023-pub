using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVolumeSlidersStartPosition : MonoBehaviour
{
    [SerializeField] SaveObject musicVolumeSaveObject;
    [SerializeField] SaveObject sfxVolumeSaveObject;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private void Start() 
    {
        musicSlider.SetValueWithoutNotify(musicVolumeSaveObject.GetValueFloat());
        sfxSlider.SetValueWithoutNotify(sfxVolumeSaveObject.GetValueFloat());
        GliderMusic.ChangeMusic.Volume(musicVolumeSaveObject.GetValueFloat());
    }
}
