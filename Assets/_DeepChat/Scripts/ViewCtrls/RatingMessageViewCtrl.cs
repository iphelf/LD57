using System.Linq;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class RatingMessageViewCtrl : MessageViewCtrlBase
    {
        [Header("Rating")] [SerializeField] private TMP_Text widthMatchResultText;
        [SerializeField] private Image widthMatchResultImage;
        [SerializeField] private TMP_Text widthMatchScoreText;
        [SerializeField] private Image emotionImage;
        [SerializeField] private TMP_Text emotionMatchScoreText;

        [SerializeField] private RatingMessageViewConfig config;

        public void SetContent(Rating rating)
        {
            var widthMatchResult = config.widthMatchResultViewConfigs.First(w => w.type == rating.WidthMatchResult);
            widthMatchResultText.text = widthMatchResult.text;
            widthMatchResultText.color = widthMatchResult.color;
            widthMatchResultImage.sprite = widthMatchResult.sprite;
            widthMatchScoreText.text = rating.WidthMatchScore >= 0
                ? $"+{rating.WidthMatchScore}"
                : rating.WidthMatchScore.ToString();
            widthMatchScoreText.color = widthMatchResult.color;
            var emotion = config.emotionViewConfigs.First(e
                => e.type == rating.NpcEmotion && e.isMatched == rating.IsEmotionMatched);
            emotionImage.sprite = emotion.sprite;
            emotionMatchScoreText.text = rating.EmotionMatchScore >= 0
                ? $"+{rating.EmotionMatchScore}"
                : rating.EmotionMatchScore.ToString();
        }
    }
}