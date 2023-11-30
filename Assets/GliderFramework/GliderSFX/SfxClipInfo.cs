using UnityEngine;

namespace GliderSFX
{
    [CreateAssetMenu(fileName = "SfxClipInfo", menuName = "GliderFramework/SfxClipInfoObject", order = 1)]
    public class SfxClipInfo : ScriptableObject
    {
        public string clipName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public float cooldownOnPlay;
        public int priority = 128;


        public SfxClipInfo(string _name, AudioClip _clip, float _volume = 0.5f, float _cooldownOnPlay = 0.25f, int _priority = 128)
        {
            clipName = _name;
            clip = _clip;
            volume = _volume;
            cooldownOnPlay = _cooldownOnPlay;
            priority = _priority;
        }
    }
}