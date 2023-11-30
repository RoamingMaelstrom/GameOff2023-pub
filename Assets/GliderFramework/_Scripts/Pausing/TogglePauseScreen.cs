using UnityEngine;
using SOEvents;
using System.Collections;

public class TogglePauseScreen : MonoBehaviour
{
    [SerializeField] BoolSOEvent pauseEvent;
    [SerializeField] SOEvent togglePauseScreenEvent;
    [SerializeField] GameObject pauseScreenContent;

    public bool active {get; private set;} = false;

    private void Awake() 
    {
        togglePauseScreenEvent.AddListener(ToggleContent);
    }

    private void ToggleContent()
    {
        pauseScreenContent.SetActive(!active);
        pauseEvent.Invoke(!active);
        active = !active;
    }
}
