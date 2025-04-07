using System;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Logic;
using _DeepChat.Scripts.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class EmoticonViewCtrl : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Sprite busySprite;
        [SerializeField] private string busyStr;

        public UnityEvent OnEmoticonShowUp;
        public Emoticon Content { get; private set; }
        public event Action CheckStateChanged;
        public bool IsVisible { get; private set; } = true;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void SetContent(Emoticon emoticon)
        {
            Content = emoticon;
            text.text = emoticon.content;
        }

        public void SetBusy()
        {
            text.text = busyStr;
            toggle.image.sprite = busySprite;
            toggle.interactable = false;
        }

        public void SetVisible(bool visible)
        {
            if (visible == true && IsVisible == false)
            {
                OnEmoticonShowUp.Invoke();
            }

            IsVisible = visible;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public bool IsChecked()
        {
            return toggle.isOn;
        }

        public void SetUnchecked()
        {
            toggle.isOn = false;
        }

        private void OnToggleValueChanged(bool isOn)
        {
            CheckStateChanged?.Invoke();
            AudioManager.PlaySfx(SfxKey.DefaultButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.PlaySfx(SfxKey.DefaultButtonHover);
        }
    }
}