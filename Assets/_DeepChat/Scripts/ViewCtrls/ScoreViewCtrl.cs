using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ScoreViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        
        public UnityEvent OnScoreChanged;
        public void UpdateScore(int score, int maxScore)
        {
            text.text = $"{score}/{maxScore}";
            OnScoreChanged?.Invoke();
        }
    }
}