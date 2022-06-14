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

        public static ExcelDatabaseColumnDeserializationResult Deserialize(string doubleSerializedColumn)
        {
            var stringList = JsonSerializer.Deserialize<List<string>>(doubleSerializedColumn);
            var result = Deserialize(stringList);
            return result;
        }

        public static ExcelDatabaseColumnDeserializationResult Deserialize(List<string> serializedColumn)
        {
            var column = new ExcelDatabaseColumn
            {
                Entries = new List<IExcelCellDatum>(),
            };
            foreach (var str in serializedColumn)
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
