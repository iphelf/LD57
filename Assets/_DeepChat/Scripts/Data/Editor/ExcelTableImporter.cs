using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace _DeepChat.Scripts.Data.Editor
{
    [ScriptedImporter(1, ExcelTableAsset.Extension)]
    public class ExcelTableImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (Path.GetFileName(ctx.assetPath).StartsWith("~$"))
                return;

            Dictionary<string, ExcelTableAsset.Sheet> sheetDict;
            try
            {
                sheetDict = LoadExcelData(ctx.assetPath);
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Failed to load file '{ctx.assetPath}' ({e})");
                return;
            }

            var asset = ScriptableObject.CreateInstance<ExcelTableAsset>();
            asset.tableName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            asset.SheetDict = sheetDict;
            ctx.AddObjectToAsset("<root>", asset);
            ctx.SetMainObject(asset);
            asset.RefreshInspectorView();
        }

        private static Dictionary<string, ExcelTableAsset.Sheet> LoadExcelData(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(fs);
            Dictionary<string, ExcelTableAsset.Sheet> sheetDict = new();
            do
            {
                var sheetName = reader.Name;
                ExcelTableAsset.Sheet sheet = new();
                List<string> columnNames = new();
                List<List<string>> rows = new();
                var isFirst = true;
                while (reader.Read())
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        var fieldCount = reader.FieldCount;
                        for (var i = 0; i < fieldCount; ++i)
                        {
                            var columnName = reader.GetString(i);
                            if (string.IsNullOrEmpty(columnName))
                            {
                                break;
                            }

                            columnNames.Add(columnName);
                        }
                    }
                    else
                    {
                        List<string> row = new(columnNames.Count);
                        for (var i = 0; i < columnNames.Count; ++i)
                        {
                            var cell = reader.GetValue(i)?.ToString() ?? string.Empty;
                            row.Add(cell);
                        }

                        rows.Add(row);
                    }
                }

                sheet.name = sheetName;
                sheet.columns = columnNames;
                sheet.ColumnOfName = new Dictionary<string, int>();
                for (var i = 0; i < columnNames.Count; ++i)
                    sheet.ColumnOfName.Add(columnNames[i], i);
                sheet.Rows = rows;
                sheetDict.Add(sheetName, sheet);
            } while (reader.NextResult());

            return sheetDict;
        }
    }
}