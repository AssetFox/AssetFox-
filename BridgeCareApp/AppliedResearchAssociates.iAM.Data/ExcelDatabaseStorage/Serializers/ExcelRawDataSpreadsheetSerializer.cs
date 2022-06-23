using System;
using System.Collections.Generic;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public static class ExcelRawDataSpreadsheetSerializer
    {
        private static readonly ExcelCellDatumValueGetter ValueGetter = new ExcelCellDatumValueGetter();
        public static string Serialize(ExcelRawDataSpreadsheet worksheet)
        {
            var worksheetObjects = new List<List<object>>();
            foreach (var column in worksheet.Columns)
            {
                var columnObjects = new List<object>();
                foreach (var entry in column.Entries)
                {
                    var value = entry.Accept(ValueGetter, Unit.Default);
                    columnObjects.Add(value);
                }
                worksheetObjects.Add(columnObjects);
            }
            var returnValue = JsonSerializer.Serialize(worksheetObjects);
            return returnValue;
        }

        public static ExcelRawDataSpreadsheetDeserializationResult Deserialize(string serializedWorksheet)
        {
            var objectLists = JsonSerializer.Deserialize<List<List<object>>> (serializedWorksheet);
            var returnValue = new ExcelRawDataSpreadsheetDeserializationResult();
            var columns = new List<ExcelRawDataColumn>();
            foreach (var objectList in objectLists)
            {
                var columnData = new List<IExcelCellDatum>();
                foreach (var obj in objectList)
                {
                    var datum = ExcelCellData.ForObject(obj);
                    columnData.Add(datum);
                }
                var deserializeColumn = ExcelRawDataColumns.WithEntries(columnData);
                columns.Add(deserializeColumn);
            }
            returnValue.Worksheet = new ExcelRawDataSpreadsheet
            {
                Columns = columns,
            };
            return returnValue;
        }
    }
}
