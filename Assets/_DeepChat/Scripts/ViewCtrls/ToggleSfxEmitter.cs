using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSfxEmitter : MonoBehaviour, IPointerEnterHandler
    {
        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private static void OnToggleValueChanged(bool value)
        {
            AudioManager.PlaySfx(SfxKey.DefaultButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_toggle)
                return;
            if (!_toggle.interactable)
                return;
            AudioManager.PlaySfx(SfxKey.DefaultButtonHover);
        }
    }
}