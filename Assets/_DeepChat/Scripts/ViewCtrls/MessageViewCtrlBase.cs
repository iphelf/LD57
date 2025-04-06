using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public abstract class MessageViewCtrlBase : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private LayoutElement layoutElement;

        public UnityEvent OnDataUpdated = new UnityEvent();

        private TextMessageViewCtrl _clone;

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