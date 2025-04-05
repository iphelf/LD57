using TMPro;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ScoreViewCtrl : MonoBehaviour
    {
        [SerializeField] private TMP_Text current;
        [SerializeField] private TMP_Text delta;

        private int _score;

        public void UpdateScore(int score)
        {
            int scoreDelta = score - _score;
            _score = score;
            current.text = _score.ToString();
            delta.text = scoreDelta.ToString();
        }
    }
}