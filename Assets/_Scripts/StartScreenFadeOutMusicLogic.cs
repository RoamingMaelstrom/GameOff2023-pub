using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenFadeOutMusicLogic : MonoBehaviour
{
    private void Start() 
    {
        GliderMusic.ChangeMusic.SwitchTrackContainer(0);
    }

    public void FadeOutMusic()
    {
        GliderMusic.ChangeMusic.VolumeFaded(0f, 1f);
    }
}
