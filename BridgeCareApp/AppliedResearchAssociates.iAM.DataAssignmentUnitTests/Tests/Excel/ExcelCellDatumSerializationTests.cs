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
            var cell = TestExcelCellData.HelloDatum();
            var serialized = ExcelCellDatumSerializer.Serialize(cell);
            var deserialized = ExcelCellDatumSerializer.Deserialize(serialized);
            var stringDeserialized = (StringExcelCellDatum)deserialized.Datum;
            Assert.Equal("hello", stringDeserialized.Value);
        }

        [Fact]
        public void DoubleCell_SerializeThenDeserialize_RoundTrips()
        {
            var cell = TestExcelCellData.PiDatum();
            var serialized = ExcelCellDatumSerializer.Serialize(cell);
            var deserialized = ExcelCellDatumSerializer.Deserialize(serialized);
            var doubleDeserialized = (DoubleExcelCellDatum)deserialized.Datum;
            Assert.Equal(Math.PI, doubleDeserialized.Value);
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
