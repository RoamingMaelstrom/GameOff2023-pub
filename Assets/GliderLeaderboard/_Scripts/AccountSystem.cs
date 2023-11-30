using UnityEngine;


namespace GliderServices
{
    public class AccountSystem : MonoBehaviour
    {
        [SerializeField] SubmitHighscore submitHighscoreObject;
        [SerializeField] RetrieveHighscores retrieveHighscoresObject;

        [SerializeField] string leaderboardId;


        static AccountSystem mainSystem;
        static bool hasMainSystem = false;

        private void Awake() 
        {
            if (!hasMainSystem) 
            {
                mainSystem = this;
                hasMainSystem = true;
            }
            else Destroy(gameObject);
        }

        private async void Start()
        {
            await ServiceConnection.InitUnityServices();

            if (!ServiceConnection.IsConnectedToNetwork())
            {
                Debug.Log("Could not find local network. Aborting SignIn.");
                return;
            }

            if (!PlayerLocalInfo.IsSetup()) PlayerLocalInfo.SetupPlayerPrefs();

            await ServiceConnection.SignIn();
            await retrieveHighscoresObject.LoadScores(leaderboardId);
        }

        private async void OnApplicationQuit() 
        {
            RenameUser(PlayerLocalInfo.PlayerName);
            await submitHighscoreObject.TrySubmitScore(leaderboardId, PlayerLocalInfo.BestScore);
            ServiceConnection.SignOut();
        }



        public async void ResetAccount()
        {
            await ServiceConnection.DeleteAccount();
            await ServiceConnection.SignIn();  // Creates a new account automatically.
            PlayerLocalInfo.SetupPlayerPrefs();
        }

        public async void SubmitHighscore(int newScore)
        {
            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
            await submitHighscoreObject.TrySubmitScore(leaderboardId, newScore);
            PlayerLocalInfo.BestScore = newScore;
            submitHighscoreObject.ResetHighestHighscore();

            await retrieveHighscoresObject.LoadScores(leaderboardId);
        }

        public void RenameUser(string newName)
        {
            PlayerLocalInfo.PlayerName = newName;
            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
        }
    }


    [System.Serializable]
    public class RetrievedScoreObject
    {
        public string playerID;
        public int rank;
        public string playerName;
        public int score;

        public RetrievedScoreObject(string _playerID, int _rank, string _playerName, int _score)
        {
            playerID = _playerID;
            rank = _rank;
            playerName = _playerName;
            score = _score;
        }
    }
}

