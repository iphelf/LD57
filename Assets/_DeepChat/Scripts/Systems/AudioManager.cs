using System.Collections.Generic;
using _DeepChat.Scripts.Data;
using UnityEngine;

namespace _DeepChat.Scripts.Systems
{
    public static class AudioManager
    {
        private static readonly Dictionary<SfxKey, AudioClip> SfxDict = new();
        private static readonly Dictionary<MusicKey, AudioClip> MusicDict = new();
        private static AudioSource _source;
        private static MusicKey _currentMusic;

        public static void Initialize(AudioConfig config, AudioSource source)
        {
            SfxDict.Clear();
            MusicDict.Clear();
            foreach (var entry in config.sfxList)
                SfxDict.Add(entry.key, entry.sfx);
            foreach (var entry in config.musicList)
                MusicDict.Add(entry.key, entry.music);
            _source = source;
            _currentMusic = MusicKey.None;
        }

        public static void PlaySfx(SfxKey key)
        {
            if (!SfxDict.TryGetValue(key, out var sfx))
            {
                Debug.LogWarning($"SFX {key} is missing.");
                return;
            }

            _source.PlayOneShot(sfx);
        }

        public static void PlayMusic(MusicKey key)
        {
            if (!MusicDict.TryGetValue(key, out var music))
            {
                Debug.LogWarning($"Music {key} is missing.");
                return;
            }

            if (key == _currentMusic) return;
            _currentMusic = key;
            _source.clip = music;
            _source.loop = true;
            _source.Play();
        }
    }
}