using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public static class ExcelDatabaseWorksheetSerializer
    {
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
