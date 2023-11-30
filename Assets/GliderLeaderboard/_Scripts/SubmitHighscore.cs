using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace GliderServices
{ 
    public class SubmitHighscore : MonoBehaviour
    {
        [SerializeField] float scoreThreshold = 1000f;
        [SerializeField] bool useHighscoreMode = true;
        [SerializeField] int maxScoreSubmissionPerPlayer = 100;
        public int bestSubmittedScore {get; private set;}

        private void Start() 
        {
            PlayerLocalInfo.SetScoreMode(useHighscoreMode);
            PlayerLocalInfo.BestScore = (int)scoreThreshold;
            bestSubmittedScore = PlayerLocalInfo.BestScore;
        }

        public async Task<bool> TrySubmitScore(string leaderboardId, int score)
        {
            int scoreMode = PlayerLocalInfo.GetScoreMode();

            ScoreCheckLog log = new();

            if (RunSubmitScoreChecks(ref log, score, scoreMode))
            {
                await SubmitScoreNoChecks(leaderboardId, score);
                LogCheck(ref log, true, VALID_SCORE_CHECK_NAME.SCORE_SUBMITTED);
                bestSubmittedScore = score;
                return true;
            }

            Debug.Log(log.GetStateAsString());
            return false;
        }

        private bool RunSubmitScoreChecks(ref ScoreCheckLog log, int score, int scoreMode)
        {
            if (!LogCheck(ref log, IsBestScore(score, scoreMode), VALID_SCORE_CHECK_NAME.BEST_SCORE)) return false;
            if (!LogCheck(ref log, NumScoreSubmissionNotTooHigh(), VALID_SCORE_CHECK_NAME.NUMBER_SUBMITTED_CAP_NOT_EXCEEDED)) return false;
            if (!LogCheck(ref log, ScoreExceedsThreshold(), VALID_SCORE_CHECK_NAME.MEETS_THRESHOLD)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsSignedIn(), VALID_SCORE_CHECK_NAME.USER_SIGNED_IN)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsValidAccessToken(), VALID_SCORE_CHECK_NAME.HAS_VALID_ACCESS_TOKEN)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsConnectedToNetwork(), VALID_SCORE_CHECK_NAME.CONNECTED_TO_LOCAL_NETWORK)) return false;
            return true;

            bool IsBestScore(int score, int scoreMode) => score * scoreMode >= PlayerLocalInfo.BestScore * scoreMode || score * scoreMode >= bestSubmittedScore * scoreMode;
            bool NumScoreSubmissionNotTooHigh() => PlayerLocalInfo.NumScoresSubmitted < maxScoreSubmissionPerPlayer;
            bool ScoreExceedsThreshold() => score * scoreMode > scoreThreshold * scoreMode;
        }

        bool LogCheck(ref ScoreCheckLog log, bool checkFunctionOutput, VALID_SCORE_CHECK_NAME checkKey)
        {
            log[checkKey] = checkFunctionOutput ? SCORE_PASS_CHECK_STATUS.TRUE : SCORE_PASS_CHECK_STATUS.FALSE;
            return checkFunctionOutput;
        }

        private async Task SubmitScoreNoChecks(string leaderboardId, int score)
        {
            Debug.Log("Submitting Score...");
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);            
            Debug.Log("Score Submitted.");
            PlayerLocalInfo.SetPlayerPrefsHighscoreNoChecks(score);
        }

        
        public void ResetHighestHighscore() => bestSubmittedScore = 0;
    }



    public class ScoreCheckLog : Dictionary<VALID_SCORE_CHECK_NAME, SCORE_PASS_CHECK_STATUS>
    {
        public ScoreCheckLog()
        {
            this[VALID_SCORE_CHECK_NAME.BEST_SCORE] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.NUMBER_SUBMITTED_CAP_NOT_EXCEEDED] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.MEETS_THRESHOLD] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.USER_SIGNED_IN] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.HAS_VALID_ACCESS_TOKEN] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.CONNECTED_TO_LOCAL_NETWORK] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_SCORE_CHECK_NAME.SCORE_SUBMITTED] = SCORE_PASS_CHECK_STATUS.FALSE;
        }

        public string GetStateAsString()
        {
            string output = "Check_Valid_Submission_Log.\n";
            foreach (var kvp in this)
            {
                string pass_status = kvp.Value switch
                {
                    SCORE_PASS_CHECK_STATUS.FALSE => "False",
                    SCORE_PASS_CHECK_STATUS.TRUE => "True",
                    _ => "Not Run",
                };

                output += string.Format("Check Name = {0}, Pass Status = {1}\n", kvp.Key, pass_status);
            }

            return output;
        }

    }

    public enum SCORE_PASS_CHECK_STATUS
    {
        FALSE = 0,
        TRUE = 1,
        NOT_RUN = 2
    }

    public enum VALID_SCORE_CHECK_NAME
    {
        BEST_SCORE,
        NUMBER_SUBMITTED_CAP_NOT_EXCEEDED,
        MEETS_THRESHOLD,
        USER_SIGNED_IN,
        HAS_VALID_ACCESS_TOKEN,
        CONNECTED_TO_LOCAL_NETWORK,
        SCORE_SUBMITTED
    }   

}
