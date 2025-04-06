using System;
using System.Collections.Generic;
using _DeepChat.Scripts.Common;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "RatingMessageViewConfig", menuName = "SO/RatingMessageViewConfig")]
    public class RatingMessageViewConfig : ScriptableObject
    {
        [Serializable]
        public class WidthMatchResultViewConfig
        {
            public WidthMatchResultType type;
            public string text;
            public Sprite sprite;
            public Color color;
        }

        public List<WidthMatchResultViewConfig> widthMatchResultViewConfigs = new();

        [Serializable]
        public class EmotionViewConfig
        {
            public EmotionType type;
            public Sprite sprite;
        }

        public List<EmotionViewConfig> emotionViewConfigs = new();

        public Sprite emotionMatchedSprite;
        public Sprite emotionNotMatchedSprite;
    }
}