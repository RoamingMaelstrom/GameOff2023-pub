using UnityEngine;
using SOEvents;

public class TogglePause : MonoBehaviour
{
    [SerializeField] BoolSOEvent updatePauseCounterEvent;
    public bool statePaused {get; private set;} = false;


    public void Toggle()
    {
        statePaused = !statePaused;
        updatePauseCounterEvent.Invoke(statePaused);
    }

}
