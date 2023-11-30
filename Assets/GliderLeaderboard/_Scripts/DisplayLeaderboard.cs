using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace GliderServices
{
    public class DisplayLeaderboard : MonoBehaviour
    {

        [SerializeField] RetrieveHighscores retrieveHighscores;
        [SerializeField] GameObject leaderboardContainer;
        [SerializeField] List<GameObject> highscoreRowPrefabs = new List<GameObject>();
        [SerializeField] GameObject leaderboardNotAvailablePanel;

        private GameObject[] highscoreGUIs;

        private bool rowsCreated = false;
        private bool checkForLeaderboardReply = false;

        private void OnEnable() 
        {
            // Todo: This is messy and could be buggy.
            if (!retrieveHighscores) retrieveHighscores = FindObjectOfType<RetrieveHighscores>();
            if (retrieveHighscores.scoreObjects.Length == 0) SetLeaderboardNotAvailable();
            else leaderboardNotAvailablePanel.SetActive(false);

            if (!rowsCreated) CreateLeaderboardRows();
            Debug.Log("Populating leaderboard");
            PopulateLeaderboard();
        }
        
        private void SetLeaderboardNotAvailable()
        {
            leaderboardNotAvailablePanel.SetActive(true);
            checkForLeaderboardReply = true;
        }

        private void FixedUpdate() 
        {
            if (checkForLeaderboardReply)
            {
                if (!retrieveHighscores) return;
                if (retrieveHighscores.scoreObjects.Length == 0) return;

                leaderboardNotAvailablePanel.SetActive(false);
                if (!rowsCreated) CreateLeaderboardRows();
                PopulateLeaderboard();
                checkForLeaderboardReply = false;
            }
        }

        private void CreateLeaderboardRows()
        {
            if (!rowsCreated)
            {
                if (retrieveHighscores.scoreObjects.Length == 0) return;
                highscoreGUIs = new GameObject[retrieveHighscores.scoreObjects.Length];
                for (int i = 0; i < highscoreGUIs.Length; i++)
                {
                    highscoreGUIs[i] = Instantiate(highscoreRowPrefabs[i % 2],
                     leaderboardContainer.transform);
                }
                rowsCreated = true;
            }
        }

        private void PopulateLeaderboard()
        {
            for (int i = 0; i < retrieveHighscores.scoreObjects.Length; i++)
            {
                RetrievedScoreObject scoreObject = retrieveHighscores.scoreObjects[i];
                
                TextMeshProUGUI[] textArray = highscoreGUIs[i].transform.GetComponentsInChildren<TextMeshProUGUI>();

                textArray[0].SetText(string.Format("#{0}", scoreObject.rank + 1));

                if (scoreObject.playerName.Length > 0)
                {
                    textArray[1].SetText(scoreObject.playerName.Substring(0, scoreObject.playerName.IndexOf("#")));
                }
                else textArray[1].SetText("");

                if (scoreObject.score > 0) textArray[2].SetText(string.Format("{0:n0}", (int)(scoreObject.score / 100)));
                else textArray[2].SetText("");
            }
        }
    }
}
