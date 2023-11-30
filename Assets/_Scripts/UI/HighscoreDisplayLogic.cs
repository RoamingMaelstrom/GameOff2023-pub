using UnityEngine;
using TMPro;

public class HighscoreDisplayLogic : MonoBehaviour
{
    [SerializeField] SaveObject highscoreSaveObject;
    [SerializeField] TextMeshProUGUI highscoreText;

    private void Start() 
    {
        int score = highscoreSaveObject.GetValueInt();
        if (score <= 0)
        {
            highscoreText.SetText("Highscore: N/A");
            return;
        }

        // Divide by 100 to adjust for fact that time is encoded in highscore
        highscoreText.SetText(string.Format("Highscore: {0:n0}", score / 100));
    }
}
