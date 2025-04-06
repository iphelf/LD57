using _DeepChat.Scripts.Common;

namespace _DeepChat.Scripts.Logic
{
    public class TurnActionResult
    {
        public bool IsTimeout;
        public float MatchWidthDiff;
        public EmotionType NpcEmotion;
        public EmotionType PlayerEmotion;
    }
}