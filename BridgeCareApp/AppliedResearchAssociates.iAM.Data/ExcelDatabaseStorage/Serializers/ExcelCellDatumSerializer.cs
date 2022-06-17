using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Visitors;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public static class ExcelCellDatumSerializer
    {
        private static readonly ExcelDatumJsonSerializationVisitor SerializationVisitor = new ExcelDatumJsonSerializationVisitor();
        public static string Serialize(IExcelCellDatum datum)
        {
            var unit = Unit.Default;
            var returnValue = datum.Accept(SerializationVisitor, unit);
            return returnValue;
        }

        private const string QuotationMark = @"""";
        private static StringExcelCellDatum DeserializeString(string serializedDatum)
        {
            if (serializedDatum.StartsWith(QuotationMark) && serializedDatum.EndsWith(QuotationMark)) {
                var value = serializedDatum.Substring(1, serializedDatum.Length - 2);
                var deserialized = new StringExcelCellDatum
                {
                    Value = value
                };
                return deserialized;
            }
            return null;
        }

        public static ExcelCellDatumDeserializationResult Deserialize(string serializedDatum)
        {
            var deserializeAsString = DeserializeString(serializedDatum);
            if (deserializeAsString != null)
            {
                return new ExcelCellDatumDeserializationResult
                {
                    Datum = deserializeAsString
                };
            }
            if (double.TryParse(serializedDatum, out double value))
            {
                var doubleDatum = new DoubleExcelCellDatum
                {
                    Value = value,
                };
                return new ExcelCellDatumDeserializationResult
                {
                    Datum = doubleDatum,
                };
            }
            if (serializedDatum.StartsWith("D"))
            {
                var remainder = serializedDatum.Substring(1);
                try
                {
                    var dateTime = JsonSerializer.Deserialize<DateTime>(remainder);
                    return new ExcelCellDatumDeserializationResult
                    {
                        Datum = ExcelCellData.DateTime(dateTime),
                    };
                }
                catch (Exception ex)
                {
                    var dateTimeMessage = $@"Cell content started with the letter ""D"", which caused the deserializer to expect a date. However, no date was found. The content is {serializedDatum}";
                    return new ExcelCellDatumDeserializationResult
                    {
                        Message = dateTimeMessage,
                    };
                }
            }
            var message = $"Deserializer failed to deserialize cell content. The value was {serializedDatum}.";
            return new ExcelCellDatumDeserializationResult
            {
                Message = message,
            };
        }
    }
}
