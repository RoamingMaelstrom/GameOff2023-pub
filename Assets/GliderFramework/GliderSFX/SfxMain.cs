using System.Collections.Generic;
using UnityEngine;
using SOEvents;
using System.Linq;


namespace GliderSFX
{
    public class SfxMain : MonoBehaviour
    {
        [SerializeField] public FloatSOEvent changeSfxVolumeEvent;
        [SerializeField] StringSOEvent playAudioBasicEvent;
        [SerializeField] SaveObject baseSfxVolumeSaveObject;

        [SerializeField] AudioListener listener;
        [SerializeField] GameObject baseAudioSourcePrefab;
        [SerializeField] int numberOfSources = 12;
        [SerializeField] ClipInfoContainer clipContainer;
        // Todo: Replace with custom class container audiosource reference and additional setting values
        [SerializeField] List<SfxAudioSourceInfo> audioSourceInfos = new();
        [SerializeField] [Range(0f, 1f)] float baseVolume;



        private void Awake() 
        {
            changeSfxVolumeEvent.AddListener(ChangeSfxVolumeEventHandler);
            playAudioBasicEvent.AddListener(PlaySfx);

            CreateAudioSources();
            if (!listener) listener = FindObjectOfType<AudioListener>();
            if (!Play.IsSetup) Play.SetSfxMainGlobalAccess(this);
            if (!Get.IsSetup) Get.SetSfxMainGlobalAccess(this);

        }

        private void PlaySfx(string clipName) => Play.Standard(clipName);

        private void Start() 
        {
            //baseVolume = GliderSave.GetSave.FloatValue(baseSfxVolumeSaveObject);
            baseVolume = baseSfxVolumeSaveObject.GetValueFloat();
        }

        private void CreateAudioSources()
        {
            for (int i = 0; i < numberOfSources; i++) 
            {
                AudioSource source = Instantiate(baseAudioSourcePrefab, transform).GetComponent<AudioSource>();
                audioSourceInfos.Add(new(i, source));
            }
        }

        private void Update() 
        {
            clipContainer.UpdateClipInfosCooldown(Time.unscaledDeltaTime);
        }

        private void FixedUpdate() 
        {
            if (listener == null) listener = FindObjectOfType<AudioListener>();
            UpdateAudioSourcePositions();
        }

        private void ChangeSfxVolumeEventHandler(float newVolume)
        {
            float oldBaseVolume = baseVolume;
            baseVolume = Mathf.Clamp(newVolume, 0f, 1f);
            baseSfxVolumeSaveObject.OverrideSet(baseVolume);
            UpdatePlayingSourcesVolume(baseVolume, oldBaseVolume);
        }

        private void UpdatePlayingSourcesVolume(float newVolume, float oldVolume)
        {
            audioSourceInfos.ForEach(sourceInfo => sourceInfo.source.volume *= newVolume / oldVolume);
        }

        public int PlayClip(string clipName, Vector3 position, float spatialBlend = 1, bool relativePos = false, bool dynamicMovement = false, Transform trackingTransform = null)
        {
            ClipInfoEntry clipInfoEntry = clipContainer.GetClipInfoEntry(clipName);

            if (clipInfoEntry.clipInfo == null) return -1;
            if (clipInfoEntry.currentCooldown > 0) return -1;

            SfxAudioSourceInfo sourceInfo = GetFreeAudioSourceAdditionalInfo();
            ConfigureAudioSource(sourceInfo, clipInfoEntry.clipInfo, position, spatialBlend, relativePos, dynamicMovement, trackingTransform);

            if (!trackingTransform) sourceInfo.source.transform.position = GetAudioSourcePosition(position, relativePos);
            else sourceInfo.source.transform.position = sourceInfo.trackingTransform.position + sourceInfo.relativePos;
            
            sourceInfo.source.Play();
            clipInfoEntry.currentCooldown = clipInfoEntry.clipInfo.cooldownOnPlay;

            return sourceInfo.ID;
        }

        private SfxAudioSourceInfo GetFreeAudioSourceAdditionalInfo()
        {
            int freeSourceIndex = audioSourceInfos.FindIndex(info => !info.source.isPlaying);
            if (freeSourceIndex > -1) return audioSourceInfos[freeSourceIndex];

            int lowestPriority = audioSourceInfos.Min(info => info.source.priority);
            return audioSourceInfos.Find(info => info.source.priority == lowestPriority);
        }

        public void ConfigureAudioSource(SfxAudioSourceInfo sourceInfo, SfxClipInfo clipInfo, Vector3 position ,float spatialBlend = 1, bool isRelativePos = false, bool dynamicMovement = false, Transform trackingTransform = null)
        {   
            sourceInfo.source.Stop();

            sourceInfo.source.clip = clipInfo.clip;
            sourceInfo.source.volume = baseVolume * clipInfo.volume;
            sourceInfo.source.priority = clipInfo.priority;
            sourceInfo.source.spatialBlend = Mathf.Clamp(spatialBlend, 0f, 1f);

            sourceInfo.clipName = clipInfo.clipName;
            sourceInfo.dynamicMovement = dynamicMovement;
            sourceInfo.isRelativePos = isRelativePos;
            sourceInfo.relativePos = isRelativePos ? position: Vector3.zero;
            sourceInfo.trackingTransform = trackingTransform;
        }

        public SfxAudioSourceInfo GetAudioSourceInfoByIndex(int index) => audioSourceInfos[index];

        private Vector3 GetAudioSourcePosition(Vector3 inputPosition, bool isRelativePos) => isRelativePos ? listener.transform.position + inputPosition : inputPosition;

        private void UpdateAudioSourcePositions()
        {
            foreach (var sourceInfo in audioSourceInfos)
            {
                if (!sourceInfo.dynamicMovement) continue;
                if (!sourceInfo.isRelativePos) return;

                if (sourceInfo.trackingTransform != null) sourceInfo.source.transform.position = sourceInfo.trackingTransform.position + sourceInfo.relativePos;
                else sourceInfo.source.transform.position = GetAudioSourcePosition(sourceInfo.relativePos, true);
            }
        }
    }


    // Todo: Potential bug: where all sources are currently in use, and a request is made to play another SFX. Could replace a SFX of the same type (has same name).
    // If something then wanted to check whether the clip had been replaced or not, it should show up as false because the new clipName is the same as the old one.
    // Could fix this by producing a UID each time a new clip is played.
    [System.Serializable]
    public class SfxAudioSourceInfo
    {
        public int ID {get; private set;}
        public AudioSource source;
        public string clipName;
        public bool dynamicMovement; 
        public bool isRelativePos;
        public Vector3 relativePos;
        public Transform trackingTransform;

        public SfxAudioSourceInfo(int id, AudioSource source)
        {
            ID = id;
            this.source = source;
            clipName = "";
            dynamicMovement = false;
            isRelativePos = false;
            relativePos = Vector3.zero;
            trackingTransform = null;
        }
    }


    public static class Get
    {
        static SfxMain sfxMainGlobalAccess;
        public static bool IsSetup{get; private set;} = false;

        public static void SetSfxMainGlobalAccess(SfxMain sfxMain)
        {
            sfxMainGlobalAccess = sfxMain;
            IsSetup = true;
        }

        public static SfxAudioSourceInfo AudioSourceInfoByIndex(int index)
        {
            if (!sfxMainGlobalAccess) return null;
            return sfxMainGlobalAccess.GetAudioSourceInfoByIndex(index);
        }
    }


    // Global Access Point for playing SFX
    public static class Play
    {
        static SfxMain sfxMainGlobalAccess;
        public static bool IsSetup{get; private set;} = false;

        public static void SetSfxMainGlobalAccess(SfxMain sfxMain)
        {
            sfxMainGlobalAccess = sfxMain;
            IsSetup = true;
        }

        public static int Standard(string clipName)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, Vector3.zero, 0, false, false);
        }

        public static int RelativeToListener(string clipName, Vector3 relativePos)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, relativePos, 1, true, true);         
        }

        public static int AtPoint(string clipName, Vector2 position)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, position, 1, false, false);
        }

        public static int AtRelativePoint(string clipName, Vector2 relativePos)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, relativePos, 1, true, false);
        }

        public static int RelativeToTransform(string clipName, Transform objectTransform, Vector3 offset)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, offset, 1, true, false, objectTransform);
        }

        public static int RelativeToTransform(string clipName, Transform objectTransform)
        {
            if (!sfxMainGlobalAccess) return -1;
            return sfxMainGlobalAccess.PlayClip(clipName, Vector3.zero, 1, true, false, objectTransform);
        }

        public static int RandomStandard(params string[] clipNames) => Standard(clipNames[Random.Range(0, clipNames.Length)]);
        public static int RandomAtCamera(params string[] clipNames) => RelativeToListener(clipNames[Random.Range(0, clipNames.Length)], Vector3.zero);
        public static int RandomAtPoint(Vector2 position, params string[] clipNames) => AtPoint(clipNames[Random.Range(0, clipNames.Length)], position);
        public static int RandomAtRelativePoint(Vector2 position, params string[] clipNames) => AtRelativePoint(clipNames[Random.Range(0, clipNames.Length)], position);
    }
}