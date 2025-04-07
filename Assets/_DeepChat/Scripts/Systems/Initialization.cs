using _DeepChat.Scripts.Data;
using UnityEngine;

namespace _DeepChat.Scripts.Systems
{
    public class Initialization : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioConfig audioConfig;

        private void Awake()
        {
            AudioManager.Initialize(audioConfig, audioSource);
        }

        private void Start()
        {
            AudioManager.PlayMusic(MusicKey.DefaultBGM);
        }
    }
}