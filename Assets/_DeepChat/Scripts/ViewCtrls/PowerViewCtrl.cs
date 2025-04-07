using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class PowerViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public UnityEvent OnValueChanged;
        public void SetValue(int value)
        {
            text.text = value.ToString();
            OnValueChanged?.Invoke();
        }
    }
}