using UnityEngine;

public class SetMainMusicLogic : MonoBehaviour
{
    [SerializeField] SaveObject volumeSaveObject;

    private void Start()
    {
        GliderMusic.ChangeMusic.SwitchTrackContainer(1);
        GliderMusic.ChangeMusic.VolumeFaded(volumeSaveObject.GetValueFloat(), 2f);
    }
    
}
