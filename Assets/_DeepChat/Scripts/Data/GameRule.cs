using System;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [Header("语料投放")] public EmoticonBank playerEmoticonBank;
        public int maxEmoticonCount = 8;
        public float shortBaseWeight = 4.5f;
        public float mediumBaseWeight = 4.0f;
        public float longBaseWeight = 1.5f;
        public float shortBonusWeight = 1.5f;
        public float mediumBonusWeight = 1.0f;
        public float longBonusWeight = 1.5f;

        public Emoticon SampleEmoticon(Func<SizeType, bool> contains)
        {
            var shortWeight = shortBaseWeight;
            if (contains(SizeType.Short))
                shortWeight += shortBonusWeight;
            var mediumWeight = mediumBaseWeight;
            if (contains(SizeType.Medium))
                mediumWeight += mediumBonusWeight;
            var longWeight = longBaseWeight;
            if (contains(SizeType.Long))
                longWeight += longBonusWeight;

            var totalWeight = shortWeight + mediumWeight + longWeight;

            var sample = Random.Range(0, totalWeight);
            if (sample < shortWeight)
            {
                return playerEmoticonBank.SampleBySize(SizeType.Short);
            }

            sample -= shortWeight;
            if (sample < mediumWeight)
            {
                return playerEmoticonBank.SampleBySize(SizeType.Medium);
            }

            return playerEmoticonBank.SampleBySize(SizeType.Long);
        }

        [Header("NPC回复")] public MessageBank npcMessageBank;
    }
}