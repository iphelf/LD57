using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform content;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private LayoutElement layoutElement;

        public UnityEvent OnDataUpdated = new UnityEvent();

        private MessageViewCtrl _clone;

        public void SetContent(string message)
        {
            text.text = message;
            OnDataUpdated?.Invoke();
        }

        public RectTransform GetContent()
        {
            return content;
        }

        public void SetLayoutEnabled(bool layoutEnabled)
        {
            layoutElement.ignoreLayout = !layoutEnabled;
        }

        public CanvasGroup GetCanvasGroup()
        {
            return canvasGroup;
        }
    }
}