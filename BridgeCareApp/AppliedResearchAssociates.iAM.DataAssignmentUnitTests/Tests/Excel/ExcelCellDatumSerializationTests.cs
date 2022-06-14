using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using Xunit;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Excel
{
    public class ExcelCellDatumSerializationTests
    {
        [Fact]
        public void StringCell_SerializeThenDeserialize_RoundTrips()
        {
            var cell = new StringExcelCellDatum
            {
                Value = "hello"
            };
            var serialized = ExcelCellDatumSerializer.Serialize(cell);
            var deserialized = ExcelCellDatumSerializer.Deserialize(serialized);
            var stringDeserialized = (StringExcelCellDatum)deserialized.Datum;
            Assert.Equal("hello", stringDeserialized.Value);
        }

        [Fact]
        public void DoubleCell_SerializeThenDeserialize_RoundTrips()
        {
            var cell = new DoubleExcelCellDatum
            {
                Value = 3.14159,
            };
            var serialized = ExcelCellDatumSerializer.Serialize(cell);
            var deserialized = ExcelCellDatumSerializer.Deserialize(serialized);
            var doubleDeserialized = (DoubleExcelCellDatum)deserialized.Datum;
            Assert.Equal(3.14159, doubleDeserialized.Value);
        }


        [Fact]
        public void DateTimeCell_SerializeThenDeserialize_RoundTrips()
        {
            var now = DateTime.Now;
            var cell = new DateTimeExcelCellDatum
            {
                Value = now,
            };
            var serialized = ExcelCellDatumSerializer.Serialize(cell);
            var deserialized = ExcelCellDatumSerializer.Deserialize(serialized);
            var dateTimeDeserialized = (DateTimeExcelCellDatum)deserialized.Datum;
            Assert.Equal(now, dateTimeDeserialized.Value);
        }
    }
}
