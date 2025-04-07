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

        private int _score;

        public void UpdateScore(int score, int maxScore, bool skipAnim = false)
        {
            text.text = $"{score}/{maxScore}";
            if (_score == score)
                return;
            var lastScore = _score;
            _score = score;
            if (skipAnim)
                return;

            AudioManager.PlaySfx(lastScore < score ? SfxKey.ScoreIncrease : SfxKey.ScoreDecrease);
            OnScoreChanged?.Invoke();
        }
    }
}