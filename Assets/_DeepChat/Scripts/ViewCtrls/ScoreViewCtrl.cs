using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ScoreViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void UpdateScore(int score, int maxScore)
        {
            text.text = $"{score}/{maxScore}";
        }
    }
}