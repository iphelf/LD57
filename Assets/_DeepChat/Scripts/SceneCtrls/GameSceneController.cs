using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Systems;
using UnityEngine;

namespace _DeepChat.Scripts.SceneCtrls
{
    public class GameSceneController : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.PlayMusic(MusicKey.GameBGM);
        }
    }
}