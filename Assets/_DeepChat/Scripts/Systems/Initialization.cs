using _DeepChat.Scripts.Data;
using UnityEngine;

namespace _DeepChat.Scripts.Systems
{
    public class Initialization : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioConfig audioConfig;

        private void Awake()
        {
            AudioManager.Initialize(audioConfig, sfxAudioSource, bgmAudioSource);
        }

        private void Start()
        {
            AudioManager.PlayMusic(MusicKey.DefaultBGM);
        }
    }
}