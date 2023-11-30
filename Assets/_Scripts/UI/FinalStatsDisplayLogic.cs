using TMPro;
using UnityEngine;

public class FinalStatsDisplayLogic : MonoBehaviour
{
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI scaleText;

    private void Update() 
    {
        if (ScaleManager.PlayerScaleGlobal >= 5)
        {
            scoreText.SetText(string.Format("Final Score: {0:n0}", scoreLogic.GetTotalScore()));
            scaleText.SetText("Scale: ∞");
        }
        else
        {
            scoreText.SetText(string.Format("Final Score: {0:n0}", scoreLogic.GetTotalScore()));
            scaleText.SetText(string.Format("Scale: {0:n0}", ScaleManager.PlayerScaleGlobal));
        }
    }
}
