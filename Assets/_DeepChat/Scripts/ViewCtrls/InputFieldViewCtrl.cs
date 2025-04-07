using System.Linq;
using _DeepChat.Scripts.Logic;
using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class InputFieldViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string hintingInputStr;
        [SerializeField] private string blockingInputStr;

        public void Clear()
        {
            text.text = string.Empty;
        }

        public void SetContent(Emoticon[] newEmoticons)
        {
            if (newEmoticons.Length == 0)
            {
                SetAsHintingInput();
                return;
            }

            var content = string.Concat(newEmoticons.Select(e => e.content));
            text.text = content;
        }

        public void SetAsHintingInput()
        {
            text.text = hintingInputStr;
        }

        public void SetAsBlockingInput()
        {
            text.text = blockingInputStr;
        }
    }
}