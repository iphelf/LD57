using System.Collections.Generic;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "EmoticonBank", menuName = "SO/EmoticonBank")]
    public class EmoticonBank : ScriptableObject
    {
        [Header("Excel")] [SerializeField] private ExcelTableAsset excelTableAsset;
        [SerializeField] private string sheetName = "WordSheet";

        [Header("Content")] public List<Emoticon> emoticons = new();

        private Dictionary<SizeType, List<Emoticon>> _emoticonsOfSize;

        [Button]
        private void ImportFromExcel()
        {
            var sheet = excelTableAsset.SheetDict[sheetName];

            emoticons.Clear();
            foreach (var row in sheet.rows)
            {
                var emoticon = new Emoticon
                {
                    content = row.cells[sheet.ColumnOfName["Content"]],
                    emotion = (EmotionType)int.Parse(row.cells[sheet.ColumnOfName["Emotion"]]),
                    size = (SizeType)int.Parse(row.cells[sheet.ColumnOfName["Size"]]),
                };
                emoticons.Add(emoticon);
            }

            OnValidate();
        }

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