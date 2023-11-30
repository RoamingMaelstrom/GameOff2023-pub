using System.Collections;
using System.Collections.Generic;
using GliderServices;
using UnityEngine;

public class HighscoreGameLogic : MonoBehaviour
{
    [SerializeField] SaveObject highscoreSave;
    [SerializeField] ScoreLogic scoreLogic;
    
    public AccountSystem accountSystem;
    private void Start() 
    {
        accountSystem = FindObjectOfType<AccountSystem>();
    }


    public void SubmitScore()
    {
        int newScore = ((int)(scoreLogic.GetTotalScore() * 100)) - (9999 - (int)scoreLogic.timer);

        int currentHighscore = highscoreSave.GetValueInt();
        if (currentHighscore >= newScore) return;
        highscoreSave.Set(newScore);

        if (accountSystem != null) accountSystem.SubmitHighscore(highscoreSave.GetValueInt());
    }
}
