using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace GliderServices
{
    public class RetrieveHighscores : MonoBehaviour
    {
        public RetrievedScoreObject[] scoreObjects;
        [SerializeField] int numScoreObjectsRetrieved = 10;
        public LeaderboardScoresPage scoresResponse {get; private set;}

        public async Task LoadScores(string leaderboardId)
        {
            GetScoresOptions scoreOptions = CreateScoreOptions(numScoreObjectsRetrieved);
            Debug.Log("Leaderboard Retrieve Initiated.");
            scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, scoreOptions);
            Debug.Log("Leaderboard Retrieve Completed.");
            scoreObjects = new RetrievedScoreObject[numScoreObjectsRetrieved];

            for (int i = 0; i < scoresResponse.Results.Count; i++)
            {
                LeaderboardEntry score = scoresResponse.Results[i];
                scoreObjects[i] = new RetrievedScoreObject(score.PlayerId, score.Rank, score.PlayerName, (int)score.Score);
            }

            AddBlankScoreObjects(numScoreObjectsRetrieved - scoresResponse.Results.Count, scoreObjects);
        }

        private GetScoresOptions CreateScoreOptions(int numScores)
        {
            GetScoresOptions scoreOptions = new GetScoresOptions();
            scoreOptions.Limit = numScores;
            scoreOptions.Offset = 0;
            return scoreOptions;
        }

        private void AddBlankScoreObjects(int numBlankObjects, RetrievedScoreObject[] scoreObjectArray)
        {
            int len = scoreObjectArray.Length;
            for (int i = len - numBlankObjects; i < scoreObjectArray.Length; i++)
            {
                scoreObjectArray[i] = new RetrievedScoreObject("#", i, "", 0);
            } 
        }
    }
}
