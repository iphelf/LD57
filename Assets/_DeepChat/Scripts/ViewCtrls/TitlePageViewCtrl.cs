using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class TitlePageViewCtrl : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button creditsButton;

        public event Action OnStartButtonClicked;
        public event Action OnCreditsButtonClicked;

        private void Awake()
        {
            startButton.onClick.AddListener(() => OnStartButtonClicked?.Invoke());
            creditsButton.onClick.AddListener(() => OnCreditsButtonClicked?.Invoke());
        }
    }
}