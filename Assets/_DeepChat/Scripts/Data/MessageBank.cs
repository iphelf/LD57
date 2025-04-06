using System.Collections.Generic;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "MessageBank", menuName = "SO/MessageBank")]
    public class MessageBank : ScriptableObject
    {
        [Header("Excel")] [SerializeField] private ExcelTableAsset excelTableAsset;
        [SerializeField] private string sheetName = "EnemySentenceSheet";

        [Header("Content")] public List<Message> messages = new();

        [Button]
        [UsedImplicitly]
        private void ImportFromExcel()
        {
            var sheet = excelTableAsset.SheetDict[sheetName];
            messages.Clear();
            foreach (var row in sheet.rows)
            {
                var message = new Message
                {
                    content = row.cells[sheet.ColumnOfName["Content"]],
                    emotion = (EmotionType)int.Parse(row.cells[sheet.ColumnOfName["Emotion"]]),
                };
                messages.Add(message);
            }
        }
    }
}