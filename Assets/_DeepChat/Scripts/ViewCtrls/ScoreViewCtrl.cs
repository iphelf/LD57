using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ScoreViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public UnityEvent OnScoreChanged;

        private int _lastScore;

        public void UpdateScore(int score, int maxScore, bool skipAnim = false)
        {
            text.text = $"{score}/{maxScore}";
            if (_lastScore == score)
                return;
            _lastScore = score;
            if (skipAnim)
                return;

            AudioManager.PlaySfx(_lastScore < score ? SfxKey.ScoreIncrease : SfxKey.ScoreDecrease);
            OnScoreChanged?.Invoke();
            _lastScore = score;
        }
    }
}