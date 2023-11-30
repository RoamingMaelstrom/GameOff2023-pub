using System.Collections.Generic;
using UnityEngine;

namespace GliderSFX
{
    public class ClipInfoContainer : MonoBehaviour
    {
        [SerializeField] List<SfxClipInfo> clipInfoObjects = new();
        Dictionary<string, ClipInfoEntry> clipInfosLookupTable = new();

        [SerializeField] bool autoLoadFromResources = true;
        [SerializeField] string loadFolderPath = "ClipInfos";

        private void Start() 
        {
            if (autoLoadFromResources) LoadClipInfosFromResources(loadFolderPath);
            clipInfosLookupTable = CreateClipInfoLookupTable();
        }

        private void LoadClipInfosFromResources(string path)
        {
            SfxClipInfo[] folderClipInfoArray = Resources.LoadAll<SfxClipInfo>(path);

            foreach (var loadedInfo in folderClipInfoArray)
            {
                if (!clipInfoObjects.Exists(o => o.clipName == loadedInfo.clipName)) clipInfoObjects.Add(loadedInfo);
            }
        }

        private Dictionary<string, ClipInfoEntry> CreateClipInfoLookupTable()
        {
            Dictionary<string, ClipInfoEntry> output = new();
            foreach (SfxClipInfo clipInfo in clipInfoObjects) 
            {
                output.Add(clipInfo.clipName, new(clipInfo));
            }
            return output;
        }

        public void UpdateClipInfosCooldown(float deltaTime)
        {
            foreach (var item in clipInfosLookupTable) item.Value.currentCooldown -= deltaTime;
        }

        public void AddClipInfo(SfxClipInfo newClipInfo)
        {           
            if (clipInfosLookupTable.TryGetValue(newClipInfo.name, out ClipInfoEntry currentEntry)) 
            {
                Debug.Log("ClipInfo Object with same name found in container. Removing old clip from container.");
                clipInfoObjects.Remove(currentEntry.clipInfo);
                clipInfosLookupTable.Remove(newClipInfo.name);
            }

            clipInfoObjects.Add(newClipInfo);
            clipInfosLookupTable.Add(newClipInfo.name, new(newClipInfo));
        }

        public ClipInfoEntry GetClipInfoEntry(string clipName)
        {
            if (clipInfosLookupTable.TryGetValue(clipName, out ClipInfoEntry entry)) return entry;
            return new ClipInfoEntry(null);
        }

        public void SetClipInfoCooldown(SfxClipInfo clipInfo) => clipInfosLookupTable[clipInfo.clipName].currentCooldown = clipInfo.cooldownOnPlay;
        public float SetClipInfoCooldown(string clipName, float newCooldown) => clipInfosLookupTable[clipName].currentCooldown = newCooldown;
        
        public float GetClipInfoCurrentCooldown(SfxClipInfo clipInfo) => clipInfosLookupTable[clipInfo.clipName].currentCooldown;
        public float GetClipInfoCurrentCooldown(string clipName) => clipInfosLookupTable[clipName].currentCooldown;

    }


    public class ClipInfoEntry
    {
        public SfxClipInfo clipInfo;
        public float currentCooldown;

        public ClipInfoEntry(SfxClipInfo clipInfo)
        {
            this.clipInfo = clipInfo;
            currentCooldown = 0;
        }
    }
}
