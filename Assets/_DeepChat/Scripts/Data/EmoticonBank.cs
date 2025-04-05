using System.Collections.Generic;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "EmoticonBank", menuName = "SO/EmoticonBank")]
    public class EmoticonBank : ScriptableObject
    {
        public List<Emoticon> emoticons = new();

        private Dictionary<SizeType, List<Emoticon>> _emoticonsOfSize;

        private void OnValidate()
        {
            _emoticonsOfSize = new Dictionary<SizeType, List<Emoticon>>();
            Debug.Log($"{nameof(EmoticonBank)} is Awake");
            foreach (var emoticon in emoticons)
            {
                if (!_emoticonsOfSize.ContainsKey(emoticon.size))
                {
                    _emoticonsOfSize.Add(emoticon.size, new List<Emoticon>());
                }

                _emoticonsOfSize[emoticon.size].Add(emoticon);
            }
        }

        public Emoticon SampleBySize(SizeType size)
        {
            var pool = _emoticonsOfSize[size];
            var index = Random.Range(0, pool.Count);
            return pool[index];
        }
    }
}