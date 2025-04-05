using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform content;

        public void SetContent(string message)
        {
            text.text = message;
        }

        public float GetContentWidth()
        {
            return content.rect.width;
        }
    }
}