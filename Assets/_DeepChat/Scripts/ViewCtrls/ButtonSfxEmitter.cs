using _DeepChat.Scripts.Data;
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

        private void OnEnable()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            var button = GetComponent<Button>();
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private static void OnButtonClicked()
        {
            AudioManager.PlaySfx(SfxKey.DefaultButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (disableHoverSound)
                return;
            AudioManager.PlaySfx(SfxKey.DefaultButtonHover);
        }
    }
}