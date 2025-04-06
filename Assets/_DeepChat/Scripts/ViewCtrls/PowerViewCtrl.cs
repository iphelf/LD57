using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class PowerViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}