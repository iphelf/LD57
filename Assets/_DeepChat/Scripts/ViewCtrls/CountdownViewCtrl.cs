using System;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class CountdownViewCtrl : MonoBehaviour
    {
        [SerializeField] private Image filler;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private bool _isRunning;
        private bool _isFadingOut;

        private float _total;
        private float _remain;
        private Action _callback;

        public void StartCountdown(float seconds, Action onTimeout)
        {
            gameObject.SetActive(true);
            _isRunning = true;
            _isFadingOut = false;
            _total = seconds;
            _remain = seconds;
            _callback = onTimeout;
        }

        public void StopCountdown()
        {
            gameObject.SetActive(false);
            _isRunning = false;
            _callback = null;
            _isFadingOut = false;
        }

        private void Update()
        {
            if (_isRunning)
            {
                _remain -= Time.deltaTime;

                if (_remain > 0.0f)
                {
                    filler.fillAmount = _remain / _total;
                    return;
                }

                filler.fillAmount = 0.0f;

                _remain = 0.0f;
                var callback = _callback;
                _isRunning = false;
                _callback = null;
                _isFadingOut = true;
                _remain = fadeOutDuration;
                callback?.Invoke();
            }
            else if (_isFadingOut)
            {
                _remain -= Time.deltaTime;
                if (_remain > 0.0f)
                    return;
                gameObject.SetActive(false);
                _isFadingOut = false;
            }
        }
    }
}