using System.Collections.Generic;
using _DeepChat.Scripts.Logic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "EmoticonBank", menuName = "SO/EmoticonBank")]
    public class EmoticonBank : ScriptableObject
    {
        public List<Emoticon> emoticons = new();
    }
}