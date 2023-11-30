using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;
using System;

public class MainMusicLogic : MonoBehaviour
{
    [Space(25)]
    [SerializeField] public FloatSOEvent changeMusicVolumeEvent;
    [SerializeField] SaveObject musicVolumeSaveObject;

    [Space(50)]

    [Header("Internals")]

    // AudioSources that play the music. Usually, only one source is playing at a time. 
    // The exception is when there is a transition between the two sources.
    [SerializeField] AudioSource musicSource1;
    [SerializeField] AudioSource musicSource2;

    [SerializeField] List<TrackContainer> trackContainers = new List<TrackContainer>();
    [SerializeField] int startTrackContainerIndex = 0;
    TrackContainer currentTrackContainer;

    // Specifies whether Clip Swapping takes place when a Clip nears it completion.
    [SerializeField] bool AUTO_PLAY_NEXT_TRACK = true;

    [Tooltip("Maximum volume that AudioSources can play at. Should remain constant during runtime.")]
    [SerializeField] [Range(0.0f, 1.0f)] float maximumVolume = 0.6f;

    [Tooltip("Volume multiplier. Manipulate this during runtime.")]
    [SerializeField] [Range(0.0f, 1.0f)] float currentVolumeMultiplier = 1f;

    [Tooltip("AudioSource that is currently playing.")]
    private AudioSource playingSource;

    VolumeTransitioner volumeTransitioner;


    Coroutine currentContainerCoroutinue;


    private void Awake() 
    {
        volumeTransitioner = new VolumeTransitioner(this);
        
        if (!GliderMusic.ChangeMusic.IsSetup) GliderMusic.ChangeMusic.SetSfxMainGlobalAccess(this);

        changeMusicVolumeEvent.AddListener(ChangeVolumeOnStart);
    }
    
    private void Start() 
    {
        currentVolumeMultiplier = musicVolumeSaveObject.GetValueFloat();
        if (playingSource == null) SetupMusicSystem();
        if (trackContainers.Count <= 0)
        {
            Debug.Log("No TrackContainers have been assigned to MainMusicLogic instance. Aborting Setup.");
            return;
        }

        playingSource = musicSource1;
        SwitchTrackContainer(trackContainers[startTrackContainerIndex]);

        playingSource.clip = currentTrackContainer.GetNextTrack();
    }

    private void StartContainerCoroutine(TrackContainerMode mode)
    {
        musicSource1.loop = false;
        musicSource2.loop = false;

        if (currentContainerCoroutinue != null) StopCoroutine(currentContainerCoroutinue);
        switch (mode)
        {
            case TrackContainerMode.MULTIPLE_TRACKS_RANDOM_PATH: currentContainerCoroutinue = StartCoroutine(RunMTAutoPlayMusicSystem()); return;
            case TrackContainerMode.MULTIPLE_TRACKS_SET_PATH: currentContainerCoroutinue = StartCoroutine(RunMTAutoPlayMusicSystem()); return;
            case TrackContainerMode.MULTIPLE_TRACKS_INDEX_PATH: currentContainerCoroutinue = StartCoroutine(RunMTAutoPlayMusicSystem()); return;
            case TrackContainerMode.MULTIPLE_TRACKS_NO_AUTOPLAY: currentContainerCoroutinue = StartCoroutine(RunInactiveMusicSystem()); return;
            case TrackContainerMode.SINGLE_TRACK_LOOPING: currentContainerCoroutinue = StartCoroutine(RunLoopingMusicSystem()); return;
            default: break;
        }
    }

    public float GetVolume() => Mathf.Clamp(currentVolumeMultiplier, 0f, 1f);
    public AudioSource GetCurrentlyPlayingAudioSource() => playingSource;

    public void ChangeVolume(float newVolume, bool setSave = false)
    {
        currentVolumeMultiplier = Mathf.Clamp(newVolume, 0f, 1f);
        if (setSave) musicVolumeSaveObject.OverrideSet(currentVolumeMultiplier);
        musicSource1.volume = GetPlayingVolume();
        musicSource2.volume = GetPlayingVolume();
    }

    public void ChangeVolumeOnStart(float newVolume)
    {
        ChangeVolume(newVolume, true);
    }

    public void ChangeVolumeFaded(float newVolume, float duration) => volumeTransitioner.TransitionToVolume(newVolume, duration); 

    public void ConfigureAutoPlay(bool autoPlay) => AUTO_PLAY_NEXT_TRACK = autoPlay;

    public void GenerateTrackContainerPath() => currentTrackContainer.GenerateNextTrackPath(currentTrackContainer.currentTrackIndex);

    public void NextTrackFadedTransition(float fadeOutTrack1Length, float fadeInTrack2Length)
    {
        if (playingSource.GetInstanceID() == musicSource1.GetInstanceID()) volumeTransitioner.FadeTransitionBetweenTracks(fadeOutTrack1Length, fadeInTrack2Length);
        else volumeTransitioner.FadeTransitionBetweenTracks(fadeOutTrack1Length, fadeInTrack2Length);
    }

    public void SwitchTrackContainerByName(string containerName) => SwitchTrackContainer(trackContainers.Find(container => container.containerName == containerName));
    public void SwitchTrackContainerByIndex(int index) => SwitchTrackContainer(trackContainers[index]);

    private void SwitchTrackContainer(TrackContainer newTrackContainer)
    {
        if (playingSource == null) SetupMusicSystem();
        playingSource.clip = null;
        currentTrackContainer = newTrackContainer;
        currentTrackContainer.RunOnEnter();
        GenerateTrackContainerPath();
        playingSource.clip = currentTrackContainer.GetNextTrack();
        playingSource.Play();
        StartContainerCoroutine(currentTrackContainer.mode);
    }

    // Returns the actual volume value that AudioSources are set to in Scenes.
    private float GetPlayingVolume() 
    {
        if (currentTrackContainer) return Mathf.Clamp(maximumVolume * currentVolumeMultiplier * currentTrackContainer.containerVolumeMultiplier, 0f, maximumVolume);
        return Mathf.Clamp(maximumVolume * currentVolumeMultiplier, 0f, maximumVolume);
    }

    IEnumerator RunMTAutoPlayMusicSystem()
    {
        yield return new WaitForFixedUpdate();
        if (!playingSource.isPlaying) playingSource.Play();
        if (playingSource.clip is null) 
        {
            AUTO_PLAY_NEXT_TRACK = false;
            Debug.Log("No AudioClips found in TrackContainer. Music System Cannot run.");
        }

        while (AUTO_PLAY_NEXT_TRACK)
        {
            float fadeOutDuration = currentTrackContainer.defaultFadeOutDuration;
            float timeUntilEndOfTrack;
            do
            {
                timeUntilEndOfTrack = playingSource.clip.length - playingSource.time;
                yield return new WaitForSecondsRealtime(Mathf.Clamp(1f, 0.005f, timeUntilEndOfTrack - fadeOutDuration)); 

            } while (timeUntilEndOfTrack > fadeOutDuration);

            NextTrack();
            yield return new WaitForSecondsRealtime(fadeOutDuration + currentTrackContainer.defaultFadeInDuration);
        }
    }

    IEnumerator RunInactiveMusicSystem()
    {

        while (true)
        {
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator RunLoopingMusicSystem()
    {
        musicSource1.loop = true;
        musicSource2.loop = true;

        yield return new WaitForFixedUpdate();

        if (!playingSource.isPlaying) playingSource.Play();
        if (playingSource.clip is null) 
        {
            AUTO_PLAY_NEXT_TRACK = false;
            Debug.Log("No AudioClips found in TrackContainer. Music System Cannot run.");
        }

        while (true)
        {
            yield return new WaitForSeconds(1f);
        }
    }



    public void NextTrack()
    {
        if (Mathf.Max(0, currentTrackContainer.defaultFadeOutDuration) + Mathf.Max(0, currentTrackContainer.defaultFadeInDuration) <= 0) RunNextTrackSimple();
        else NextTrackFadedTransition(currentTrackContainer.defaultFadeOutDuration, currentTrackContainer.defaultFadeInDuration);
    }

    public void RunNextTrackSimple()
    {
        AudioSourceSwapper.SwapSourceInPlace(ref playingSource, ref musicSource1, ref musicSource2, true);
        playingSource.clip = currentTrackContainer.GetNextTrack();
        if (!playingSource.isPlaying) playingSource.Play();
    }

    void SetupAudioSourcesVolume()
    {
        musicSource1.volume = GetPlayingVolume();
        musicSource2.volume = GetPlayingVolume();
    }

    // Generates a random order of tracks, sets music players to normalVolume, sets playingSource to musicSource1
    void SetupMusicSystem()
    {
        SetupAudioSourcesVolume();
        playingSource = musicSource1;
        if (trackContainers.Count == 0) return;
        currentTrackContainer = trackContainers[startTrackContainerIndex];
    }

    public void SetTrack(int index)
    {
        playingSource.clip = currentTrackContainer.GetTrackByIndex(index);
        if (!playingSource.isPlaying) playingSource.Play();
    }
}



public class VolumeTransitioner
{
    public VolumeTransitioner(MainMusicLogic _mainMusicLogic)
    {
        mainMusicLogic = _mainMusicLogic;
    }

    MainMusicLogic mainMusicLogic;

    private float startVolume;
    private float endVolume;
    private float transitionDuration;

    Coroutine currentFadeTransitionBetweenTracksRoutine;
    Coroutine currentVolumeTransitionRoutine;

    public void FadeTransitionBetweenTracks(float fadeOutLength, float fadeInLength)
    {
        if (currentFadeTransitionBetweenTracksRoutine != null) mainMusicLogic.StopCoroutine(currentFadeTransitionBetweenTracksRoutine);
        if (currentVolumeTransitionRoutine != null) mainMusicLogic.StopCoroutine(currentVolumeTransitionRoutine);

        currentFadeTransitionBetweenTracksRoutine = mainMusicLogic.StartCoroutine(FadeTransitionBetweenTracksRoutine(fadeOutLength, fadeInLength));
    }

    IEnumerator FadeTransitionBetweenTracksRoutine(float fadeOutLength, float fadeInLength)
    {
        startVolume = mainMusicLogic.GetVolume();
        endVolume = 0;
        transitionDuration = fadeOutLength;
        yield return mainMusicLogic.StartCoroutine(TransitionToVolume());

        mainMusicLogic.RunNextTrackSimple();

        endVolume = startVolume;
        startVolume = 0;
        transitionDuration = fadeInLength;
        yield return mainMusicLogic.StartCoroutine(TransitionToVolume());
    }

    public void TransitionToVolume(float targetVolume, float duration)
    {
        if (currentVolumeTransitionRoutine != null) mainMusicLogic.StopCoroutine(currentVolumeTransitionRoutine);

        startVolume = mainMusicLogic.GetVolume();
        endVolume = targetVolume;
        transitionDuration = duration;
        
        currentVolumeTransitionRoutine = mainMusicLogic.StartCoroutine(TransitionToVolume());
    }

    IEnumerator TransitionToVolume()
    {
        float rateOfChange = (endVolume - startVolume) * 0.02f / transitionDuration;
        float currentVolume = startVolume;

        while (Mathf.Abs(endVolume - currentVolume) > Mathf.Abs(rateOfChange))
        {
            currentVolume += rateOfChange;
            mainMusicLogic.ChangeVolume(currentVolume, false);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        yield return null;
    }
}



public class AudioSourceSwapper
{
    public static void SwapSourceInPlace(ref AudioSource playingSource, ref AudioSource audioSource1, ref AudioSource audioSource2, bool copyVolume = true)
    {
        if (copyVolume) 
        {
            audioSource1.volume = playingSource.volume;
            audioSource2.volume = playingSource.volume;
        }

        playingSource.Stop();

        if (playingSource.GetInstanceID() == audioSource1.GetInstanceID()) playingSource = audioSource2;
        else playingSource = audioSource1;

    }
}



namespace GliderMusic
{   
    public static class ChangeMusic
    {
        private static MainMusicLogic musicMainGlobalAccess;
        public static bool IsSetup{get; private set;} = false;

        public static void SetSfxMainGlobalAccess(MainMusicLogic mainMusicLogic)
        {
            musicMainGlobalAccess = mainMusicLogic;
            IsSetup = true;
        }

        public static void ConfigureAutoPlayNextTrack(bool autoPlay)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.ConfigureAutoPlay(autoPlay);
        }

        public static void GenerateTrackContainerPath()
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.GenerateTrackContainerPath();
        }

        public static void NextTrack()
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.NextTrack();
        }

        public static void NextTrackNoFade()
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.RunNextTrackSimple();
        }

        public static void NextTrackFaded(float fadeOutDuration, float fadeInDuration)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.NextTrackFadedTransition(fadeOutDuration, fadeInDuration);
        }

        public static void SetTrackByIndex(int index)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.SetTrack(index);
        }

        public static void SwitchTrackContainer(int newTrackContainerIndex)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.SwitchTrackContainerByIndex(newTrackContainerIndex);
        }

        public static void SwitchTrackContainer(string newTrackContainerName)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.SwitchTrackContainerByName(newTrackContainerName);
        }

        public static void Volume(float newVolume)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.ChangeVolume(newVolume, true);
        }

        public static void VolumeFaded(float newVolume, float fadeDuration)
        {
            if (!musicMainGlobalAccess) return;
            musicMainGlobalAccess.ChangeVolumeFaded(newVolume, fadeDuration);
        }

    }
}
