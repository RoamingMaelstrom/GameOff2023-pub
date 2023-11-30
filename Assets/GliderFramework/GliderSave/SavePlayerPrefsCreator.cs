using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GliderSave
{
    public class SavePlayerPrefsCreator : MonoBehaviour
    {
        [SerializeField] string loadFolderPath = "SaveObjects";

        private void Awake() 
        {
            SaveObject[] saveObjectArray = Resources.LoadAll<SaveObject>(loadFolderPath);
            int creationCounter = 0;

            foreach (var saveObject in saveObjectArray)
            {
                CreateSave.Create(saveObject);
                creationCounter++;
            }

            if (creationCounter > 0) Debug.Log(string.Format("Created {0} new PlayerPrefs records", creationCounter));
        }
    }
}
