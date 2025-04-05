using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameRule", menuName = "SO/GameRule")]
    public class GameRule : ScriptableObject
    {
        [Header("匹配度")] public float perfectMatchBound = 20.0f;
        public float goodMatchBound = 50.0f;
        public int perfectMatchScore = 20;
        public int goodMatchScore = 10;
        public int badMatchScore = -10;

        public int GetScoreChange(float distance)
        {
            if (distance < perfectMatchBound)
                return perfectMatchScore;
            else if (distance < goodMatchBound)
                return goodMatchScore;
            else
                return badMatchScore;
        }
    }
}