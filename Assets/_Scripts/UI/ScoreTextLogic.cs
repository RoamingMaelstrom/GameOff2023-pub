using UnityEngine;
using TMPro;

public class ScoreTextLogic : MonoBehaviour
{
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] TextMeshProUGUI scoreText;

    private void FixedUpdate() 
    {
        scoreText.SetText(string.Format("Score\n{0:n0}", scoreLogic.GetTotalScore()));
    }
}
