using UnityEngine;

namespace GliderServices
{
    public static class PlayerLocalInfo
    {
        public static bool IsSetup() => PlayerPrefs.GetInt("PREFS_SETUP", 0) == 1;

        public static int BestScore
        {
            get {return PlayerPrefs.GetInt("best-score");}

            set {
                    int bestScore = PlayerPrefs.GetInt("best-score");
                    if (value > bestScore && IsHighscoreMode()) SetPlayerPrefsHighscoreNoChecks(value);
                    else if (value < bestScore) SetPlayerPrefsHighscoreNoChecks(value);
                }
        }

        public static int NumScoresSubmitted
        {
            get => PlayerPrefs.GetInt("num-scores-submitted");
            private set {;}
        }

        public static void IncrementNumScoresSubmitted()
        {
            PlayerPrefs.SetInt("num-scores-submitted", PlayerPrefs.GetInt("num-scores-submitted") + 1);
        }

        public static string PlayerName
        {
            get => PlayerPrefs.GetString("player-name");
            set => PlayerPrefs.SetString("player-name", value);
        }

        public static bool IsHighscoreMode() => PlayerPrefs.GetInt("score-mode", 1) == 1;
        public static int GetScoreMode() => PlayerPrefs.GetInt("score-mode", 1);
        public static void SetScoreMode(bool isHighscoreMode)
        {
            if (isHighscoreMode) PlayerPrefs.SetInt("score-mode", 1);
            else PlayerPrefs.SetInt("score-mode", -1);
        }

        public static void SetupPlayerPrefs()
        {
            PlayerPrefs.SetInt("best-score", 0);
            PlayerPrefs.SetString("player-name", GenerateRandomNameLowerCase(8, 12));
            PlayerPrefs.SetInt("num-scores-submitted", 0);
            PlayerPrefs.SetInt("score-mode", 1);
            PlayerPrefs.SetInt("PREFS_SETUP", 1);
        }

        public static string GenerateRandomNameLowerCase(int minLen = 8, int maxLen=20)
        {
            string output = "";
            for (int i = 0; i < Random.Range(minLen, maxLen); i++)
            {
                output += (char) Random.Range(97, 123);
            }
            return output;
        }

        public static void SetPlayerPrefsHighscoreNoChecks(int highscore)
        {
            PlayerPrefs.SetInt("best-score", highscore);
            IncrementNumScoresSubmitted();
        }
    }
}