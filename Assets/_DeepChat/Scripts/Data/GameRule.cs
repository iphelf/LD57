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
        public int terribleMatchScore = -15;
        public int emotionBonusMatchScore = 5;

        public Rating RateTurnActionResult(TurnActionResult actionResult)
        {
            Rating rating = new();
            if (actionResult.IsTimeout)
            {
                rating.WidthMatchResult = WidthMatchResultType.Terrible;
                rating.WidthMatchScore = terribleMatchScore;
            }
            else
            {
                var distance = Mathf.Abs(actionResult.MatchWidthDiff);
                if (distance < perfectMatchBound)
                {
                    rating.WidthMatchResult = WidthMatchResultType.Perfect;
                    rating.WidthMatchScore = perfectMatchScore;
                }
                else if (distance < goodMatchBound)
                {
                    rating.WidthMatchResult = WidthMatchResultType.Good;
                    rating.WidthMatchScore = goodMatchScore;
                }
                else
                {
                    rating.WidthMatchResult = WidthMatchResultType.Bad;
                    rating.WidthMatchScore = badMatchScore;
                }
            }

            rating.NpcEmotion = actionResult.NpcEmotion;
            rating.IsEmotionMatched = actionResult.PlayerEmotion == actionResult.NpcEmotion;
            rating.EmotionMatchScore = rating.IsEmotionMatched ? emotionBonusMatchScore : 0;
            return rating;
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

        [Header("NPC消息")] public MessageBank npcMessageBank;
        public float npcMaxWaitDuration = 5.0f;
        public int minConsecutiveUniqueSampleCount = 7;
    }
}