using System;
using _DeepChat.Scripts.Common;

namespace _DeepChat.Scripts.Logic
{
    [Serializable]
    public class Emoticon
    {
        public string content;
        public EmotionType emotion;
        public SizeType size;
    }
}