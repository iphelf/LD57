using System;
using System.Collections.Generic;
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
            public List<List<string>> Rows;
        }

        public Dictionary<string, Sheet> SheetDict;

        [Serializable]
        public class SheetEntry
        {
            public string name;
            public List<string> columns;
            public List<RowEntry> rows;
        }

        [Serializable]
        public class RowEntry
        {
            public List<string> cells;
        }

        [SerializeField, ReadOnly] private List<SheetEntry> sheets;

        public void RefreshInspectorView()
        {
            sheets = new List<SheetEntry>();
            foreach (var (sheetName, sheetData) in SheetDict)
            {
                var sheetEntry = new SheetEntry
                {
                    name = sheetName,
                    columns = sheetData.columns,
                    rows = new List<RowEntry>()
                };
                foreach (var row in sheetData.Rows)
                    sheetEntry.rows.Add(new RowEntry { cells = row });
                sheets.Add(sheetEntry);
            }
        }
    }
}