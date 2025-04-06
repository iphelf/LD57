using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class CreditsPageViewCtrl : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        public event Action OnClose;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                Destroy(gameObject);
            });
        }
    }
}