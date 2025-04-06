using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class CountdownViewCtrl : MonoBehaviour
    {
        [SerializeField] private Image filler;

        private bool _isRunning;

        private float _total;
        private float _remain;
        private Action _callback;

        public void StartCountdown(float seconds, Action onTimeout)
        {
            gameObject.SetActive(true);
            _isRunning = true;
            _total = seconds;
            _remain = seconds;
            _callback = onTimeout;
        }

        public void StopCountdown()
        {
            gameObject.SetActive(false);
            _isRunning = false;
            _callback = null;
        }

        private void Update()
        {
            if (!_isRunning)
                return;
            _remain -= Time.deltaTime;

            if (_remain > 0.0f)
            {
                filler.fillAmount = _remain / _total;
                return;
            }

            filler.fillAmount = 0.0f;

            _remain = 0.0f;
            var callback = _callback;
            StopCountdown();
            callback?.Invoke();
        }
    }
}