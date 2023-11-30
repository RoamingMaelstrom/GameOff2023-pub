using UnityEngine;
using UnityEditor;

namespace GliderSFX
{
    public class SfxClipInfoCreator : MonoBehaviour 
    {
        [SerializeField] string resourceFolderPath = "Assets/GliderFramework/Resources/";
        [SerializeField] float defaultCooldown = 0.25f;
        [SerializeField] [Range(0f, 1f)] float defaultVolume = 1f;
        [SerializeField] [Range(0, 256)] int defaultPriority = 128;

        private void Awake() 
        {
            AudioClip[] clipArray = Resources.LoadAll<AudioClip>("Clips");
            SfxClipInfo[] clipInfoArray = Resources.LoadAll<SfxClipInfo>("ClipInfos");

            bool skipCreation;
            int creationCounter = 0;

            foreach (var clip in clipArray)
            {
                skipCreation = false;
                foreach (var clipInfo in clipInfoArray)
                {
                    if (clipInfo.clipName == clip.name.Replace(" ", "_")) 
                    {
                        skipCreation = true;
                        break;
                    }
                }

                if (!skipCreation) 
                {
                    CreateNewSfxClipInfo(clip);
                    creationCounter ++;
                }
            }

            if (creationCounter > 0) Debug.Log(string.Format("Created {0} new SfxClipInfo Objects", creationCounter));
        }

        public void CreateNewSfxClipInfo(AudioClip clip)
        {
            SfxClipInfo clipInfo = ScriptableObject.CreateInstance<SfxClipInfo>();
            clipInfo.clip = clip;
            clipInfo.clipName = clip.name.Replace(" ", "_").ToLower();
            clipInfo.cooldownOnPlay = defaultCooldown;
            clipInfo.volume = defaultVolume;
            clipInfo.priority = defaultPriority;

            AssetDatabase.CreateAsset(clipInfo, resourceFolderPath + "ClipInfos/" + clipInfo.clipName + ".asset");
            AssetDatabase.SaveAssets ();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = clipInfo;

            Debug.Log(string.Format("Created new SfxClipInfo Object with name {0}", clipInfo.clipName));
        }
    }
}
