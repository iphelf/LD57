using System.Collections.Generic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "EmoticonBank", menuName = "SO/EmoticonBank")]
    public class EmoticonBank : ScriptableObject
    {
        public List<string> emoticons = new();
    }
}