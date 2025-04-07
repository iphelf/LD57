using System;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class EndingDialogViewCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject happyEnd;
        [SerializeField] private GameObject badEnd;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        public event Action OnRestartButtonClicked;
        public event Action OnExitButtonClicked;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
            exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClick);
            exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        public void OpenEnding(bool isHappy)
        {
            happyEnd.SetActive(isHappy);
            badEnd.SetActive(!isHappy);
            gameObject.SetActive(true);
            AudioManager.PlaySfx(isHappy ? SfxKey.EndHappy : SfxKey.EndBad);
        }

        private void OnRestartButtonClick()
        {
            gameObject.SetActive(false);
            OnRestartButtonClicked?.Invoke();
        }

        private void OnExitButtonClick()
        {
            OnExitButtonClicked?.Invoke();
        }
    }
}