using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using Xunit;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Excel
{
    public class ExcelColumnSerializationTests
    {
        [Fact]
        public static void ExcelColumn_SerializeThenDeserialize_RoundTrips()
        {
            var now = DateTime.Now;
            var nowDatum = ExcelCellData.DateTime(now);
            var data = new List<IExcelCellDatum>
            {
                TestExcelCellData.HelloDatum(),
                TestExcelCellData.HelloCommaWorldDatum(),
                TestExcelCellData.PiDatum(),
                TestExcelCellData.PunctuationDatum(),
                nowDatum
            };
            var column = new ExcelDatabaseColumn
            {
                Entries = data,
            };
            var serialized = ExcelDatabaseColumnSerializer.Serialize(column);
            var deserialized = ExcelDatabaseColumnSerializer.Deserialize(serialized);
            Assert.Equal(column.Entries.Count, deserialized.Column.Entries.Count);
            Assert.Null(deserialized.Message);
            var reserialized = ExcelDatabaseColumnSerializer.Serialize(deserialized.Column);
            Assert.Equal(serialized, reserialized);
        }
    }
}
