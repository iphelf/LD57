using System;
using System.Collections.Generic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    public enum SfxKey
    {
        None = 0,
        DefaultButtonClick = 1,
        DefaultButtonHover = 2,
        ReceiveMessage = 3,
        SendMessage = 4,
        MatchPerfect = 5,
        MatchGood = 6,
        MatchBad = 7,
        MatchTerrible = 8,
        ScoreIncrease = 9,
        ScoreDecrease = 10,
        EndHappy = 11,
        EndBad = 12,
        Tips = 13,
    }

    public enum MusicKey
    {
        None = 0,
        DefaultBGM = 1,
    }

    [CreateAssetMenu(menuName = "SO/AudioConfig", fileName = "AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [Range(0.0f, 1.0f)] public float musicVolume = 1.0f;
        [Range(0.0f, 1.0f)] public float sfxVolume = 1.0f;
        public List<MusicEntry> musicList;
        public List<SfxEntry> sfxList;

        [Serializable]
        public struct MusicEntry
        {
            public MusicKey key;
            public AudioClip music;
        }

        [Serializable]
        public struct SfxEntry
        {
            public SfxKey key;
            public AudioClip sfx;
        }
    }
}