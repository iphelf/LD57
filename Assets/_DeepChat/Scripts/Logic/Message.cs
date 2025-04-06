using System;
using _DeepChat.Scripts.Common;

namespace _DeepChat.Scripts.Logic
{
    [Serializable]
    public class Message
    {
        public string content;
        public EmotionType emotion;
    }
}