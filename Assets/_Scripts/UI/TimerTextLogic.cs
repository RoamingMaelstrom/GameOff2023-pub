using TMPro;
using UnityEngine;

public class TimerTextLogic : MonoBehaviour
{
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] TextMeshProUGUI timerText;

    private void FixedUpdate() 
    {
        int minutes = (int)(scoreLogic.timer / 60);
        int seconds = (int)scoreLogic.timer % 60;

        if (seconds >= 10) timerText.SetText(string.Format("{0}:{1}", minutes, seconds));
        else if (seconds > 0) timerText.SetText(string.Format("{0}:0{1}", minutes, seconds));
        else timerText.SetText(string.Format("{0}:00", minutes));
    }
}
