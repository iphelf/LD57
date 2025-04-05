using _DeepChat.Scripts.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class EmoticonViewCtrl : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TMP_Text text;

        public Emoticon Content { get; private set; }

        public void SetContent(Emoticon emoticon)
        {
            Content = emoticon;
            text.text = emoticon.content;
        }

        public bool IsChecked()
        {
            return toggle.isOn;
        }
    }
}