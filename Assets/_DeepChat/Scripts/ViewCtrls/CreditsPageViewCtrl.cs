using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class CreditsPageViewCtrl : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform membersWheel;
        [SerializeField] private float rotationSpeed = -30.0f;

        public event Action OnClose;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                Destroy(gameObject);
            });
        }

        private void Update()
        {
            var degrees = rotationSpeed * Time.deltaTime;
            membersWheel.Rotate(0.0f, 0.0f, degrees);
        }
    }
}