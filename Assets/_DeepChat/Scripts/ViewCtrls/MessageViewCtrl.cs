using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform content;
        
        public UnityEvent OnDataUpdated = new UnityEvent();
        public void SetContent(string message)
        {
            text.text = message;
            OnDataUpdated?.Invoke();
        }

        public float GetContentWidth()
        {
            return content.rect.width;
        }
    }
}