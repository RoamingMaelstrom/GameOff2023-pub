using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "TrackContainer", menuName = "GliderFramework/TrackContainer", order = 2)]
public class TrackContainer : ScriptableObject
{
    public string containerName;

    [Tooltip("Tracks that will be selected")]
    public List<AudioClip> tracks = new List<AudioClip>();

    public List<int> trackPath = new List<int>();

    public int currentTrackPathIndex {get; private set;} = -1;
    public int currentTrackIndex {get; private set;} = -1;
    public float containerVolumeMultiplier = 1f;
    public float defaultFadeOutDuration = 1f;
    public float defaultFadeInDuration = 1f;
    public TrackContainerMode mode;

    public void RunOnEnter()
    {
        currentTrackPathIndex = -1;
        containerVolumeMultiplier = 1f;
        GenerateNextTrackPath();
    }

    public AudioClip GetNextTrack()
    {
        if (tracks.Count == 0) return null;

        currentTrackPathIndex ++;
        if (currentTrackPathIndex >= trackPath.Count) 
        {
            GenerateNextTrackPath(trackPath[^1]);
            currentTrackPathIndex = 0;
        }

        currentTrackIndex = trackPath[currentTrackPathIndex];
        return tracks[currentTrackIndex];
    }

    public void GenerateNextTrackPath(int indexToBanFromStart = -1)
    {
        List<int> newTrackPath = TrackPathGenerator.GetTrackPath(mode, tracks.Count, indexToBanFromStart);
        if (newTrackPath[0] == -1000) return;
        trackPath = newTrackPath;
    }

    public AudioClip GetTrackByIndex(int index) => tracks[index];
}



// Todo: Implement other track path generation methods
public class TrackPathGenerator
{
    public static List<int> GetTrackPath(TrackContainerMode mode, int numTracks, int indexToBanFromStart = -1)
    {
        switch (mode)
        {
            case TrackContainerMode.MULTIPLE_TRACKS_RANDOM_PATH: return GetRandomTrackPath(numTracks, indexToBanFromStart);
            case TrackContainerMode.MULTIPLE_TRACKS_INDEX_PATH: return GetIndexTrackPath(numTracks);
            case TrackContainerMode.SINGLE_TRACK_LOOPING: return new List<int>{0};
            default: return new List<int>{-1000};
        }
    }

    private static List<int> GetIndexTrackPath(int numTracks)
    {
        List<int> output = new();
        for (int i = 0; i < numTracks; i++) output.Add(i);
        return output;
    }

    private static List<int> GetRandomTrackPath(int numTracks, int indexToBanFromStart = -1)
    {
        // Indexes remaining that can be selected as the next trackPath index.
        List<int> trackIndexesRemaining = new();

        for (int i = 0; i < numTracks; i++) trackIndexesRemaining.Add(i);

        List<int> newTrackPath = new();
        int randomTrackIndex;

        for (int i = 0; i < numTracks; i++)
        {
            randomTrackIndex = Random.Range(0, trackIndexesRemaining.Count);
            newTrackPath.Add(trackIndexesRemaining[randomTrackIndex]);
            trackIndexesRemaining.RemoveAt(randomTrackIndex);
        }

        if (newTrackPath[0] == indexToBanFromStart) SwapFirstIndexWithRandomIndex(newTrackPath);

        return newTrackPath;
    }

    static void SwapFirstIndexWithRandomIndex(List<int> items)
    {
        int newRandomIndex = Random.Range(1, items.Count);
        int tempValue = items[newRandomIndex];
        items[newRandomIndex] = items[0];
        items[0] = tempValue;
    }
}


public enum TrackContainerMode
{
    MULTIPLE_TRACKS_RANDOM_PATH = 0,
    MULTIPLE_TRACKS_SET_PATH = 1,
    MULTIPLE_TRACKS_INDEX_PATH = 2,
    MULTIPLE_TRACKS_NO_AUTOPLAY = 3,
    SINGLE_TRACK_LOOPING = 10
}