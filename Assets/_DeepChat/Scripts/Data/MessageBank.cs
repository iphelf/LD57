using System.Collections.Generic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "MessageBank", menuName = "SO/MessageBank")]
    public class MessageBank : ScriptableObject
    {
        public List<string> messages = new();
    }
}