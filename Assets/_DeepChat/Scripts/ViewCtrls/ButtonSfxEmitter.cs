﻿using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    [RequireComponent(typeof(Button))]
    public class ButtonSfxEmitter : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private bool disableHoverSound;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private static void OnButtonClicked()
        {
            AudioManager.PlaySfx(SfxKey.DefaultButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (disableHoverSound)
                return;
            if (!_button)
                return;
            if (!_button.interactable)
                return;
            AudioManager.PlaySfx(SfxKey.DefaultButtonHover);
        }
    }
}