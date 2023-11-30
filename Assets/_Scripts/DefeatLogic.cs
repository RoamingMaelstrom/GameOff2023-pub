using UnityEngine;
using SOEvents;

public class DefeatLogic : MonoBehaviour
{
    [SerializeField] SaveObject musicVolumeSaved;
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] GameObject defeatPanelContent;

    private void Awake() 
    {
        defeatEvent.AddListener(OpenDefeatPanel);
    }

    private void OpenDefeatPanel()
    {
        defeatPanelContent.SetActive(true);
        GliderMusic.ChangeMusic.VolumeFaded(musicVolumeSaved.GetValueFloat() / 2f, 1f);
        Time.timeScale = 0;
    }
}
