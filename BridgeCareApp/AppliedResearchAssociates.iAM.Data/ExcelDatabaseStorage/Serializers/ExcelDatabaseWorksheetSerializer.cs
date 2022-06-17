using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public static class ExcelDatabaseWorksheetSerializer
    {
        public static string Serialize(ExcelDatabaseWorksheet worksheet)
        {
            var serializeColumns = new List<List<string>>();
            foreach (var column in worksheet.Columns)
            {
                var serializeColumn = ExcelDatabaseColumnSerializer.Serialize(column);
                serializeColumns.Add(serializeColumn);
            }
            var returnValue = JsonSerializer.Serialize(serializeColumns);
            return returnValue;
        }

        public static ExcelDatabaseWorksheetDeserializationResult Deserialize(string serializedWorksheet)
        {
            var stringLists = JsonSerializer.Deserialize<List<List<string>>> (serializedWorksheet);
            var returnValue = new ExcelDatabaseWorksheetDeserializationResult();
            var columns = new List<ExcelDatabaseColumn>();
            foreach (var stringList in stringLists)
            {
                var deserializeColumn = ExcelDatabaseColumnSerializer.Deserialize(stringList);
                if (deserializeColumn.Message!=null)
                {
                    returnValue.Message = deserializeColumn.Message;
                    return returnValue;
                }
                columns.Add(deserializeColumn.Column);
            }
            returnValue.Worksheet = new ExcelDatabaseWorksheet
            {
                Columns = columns,
            };
            return returnValue;
        }
    }
}
