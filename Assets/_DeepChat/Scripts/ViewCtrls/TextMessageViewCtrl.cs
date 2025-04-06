using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class TextMessageViewCtrl : MessageViewCtrlBase
    {
        [Header("Text")] [SerializeField] private TMP_Text text;

        public void SetContent(string message)
        {
            text.text = message;
            OnDataUpdated?.Invoke();
        }
    }
}