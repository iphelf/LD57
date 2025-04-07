using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MenuDialogViewCtrl : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private GameObject content;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button closeButton;

        public event Action OnHelpButtonClicked;
        public event Action OnExitButtonClicked;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            helpButton.onClick.AddListener(OnHelpButtonClick);
            exitButton.onClick.AddListener(OnExitButtonClick);
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        private void OnToggleValueChanged(bool value)
        {
            content.SetActive(value);
        }

        private void OnHelpButtonClick()
        {
            toggle.isOn = false;
            OnHelpButtonClicked?.Invoke();
        }

        private void OnExitButtonClick()
        {
            toggle.isOn = false;
            OnExitButtonClicked?.Invoke();
        }

        private void OnCloseButtonClick()
        {
            toggle.isOn = false;
        }
    }
}