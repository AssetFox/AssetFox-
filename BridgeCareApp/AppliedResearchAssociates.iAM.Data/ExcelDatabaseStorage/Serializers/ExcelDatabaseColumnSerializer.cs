using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public static class ExcelDatabaseColumnSerializer
    {
        public static string Serialize(ExcelDatabaseColumn column)
        {
            var serializeValues = column.Entries.Select(e => ExcelCellDatumSerializer.Serialize(e)).ToList();
            var returnValue = JsonSerializer.Serialize(serializeValues);
            return returnValue;
        }

        public static ExcelDatabaseColumnDeserializationResult Deserialize(string serializedColumn)
        {
            var deserializeValues = JsonSerializer.Deserialize<List<string>>(serializedColumn);
            var column = new ExcelDatabaseColumn();
            foreach (var str in deserializeValues)
            {
                var deserializeStr = ExcelCellDatumSerializer.Deserialize(str);
                if (deserializeStr.Datum != null)
                {
                    column.Entries.Add(deserializeStr.Datum);
                }
                else
                {
                    return new ExcelDatabaseColumnDeserializationResult
                    {
                        Message = deserializeStr.Message,
                    };
                }
            }
            return new ExcelDatabaseColumnDeserializationResult
            {
                Column = column,
            };
        }
    }
}
