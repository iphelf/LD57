using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void SetContent(string message)
        {
            text.text = message;
        }
    }
}