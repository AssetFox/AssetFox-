using System;
using System.Collections.Generic;
using System.Text;
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
            var message = $"Deserializer failed to deserialize cell content. The value was {serializedDatum}.";
            return new ExcelCellDatumDeserializationResult
            {
                Message = message,
            };
        }
    }
}
