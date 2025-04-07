using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    public class ExcelTableAsset : ScriptableObject
    {
        public const string Extension = "xlsx";
        public string tableName;

        [Serializable]
        public class Sheet
        {
            public string name;
            public List<string> columns;
            public Dictionary<string, int> ColumnOfName;
            public List<RowEntry> rows;
        }

        [Serializable]
        public class RowEntry
        {
            public List<string> cells;
        }

        [ReadOnly] public List<Sheet> sheets;

        public Dictionary<string, Sheet> SheetDict;

        private void OnValidate()
        {
            SheetDict = sheets.ToDictionary(sheet => sheet.name, sheet => sheet);
            foreach (var sheet in sheets)
            {
                sheet.ColumnOfName = new Dictionary<string, int>();
                for (var i = 0; i < sheet.columns.Count; ++i)
                    sheet.ColumnOfName.Add(sheet.columns[i], i);
            }
        }

        private void OnEnable()
        {
            OnValidate();
        }
    }
}