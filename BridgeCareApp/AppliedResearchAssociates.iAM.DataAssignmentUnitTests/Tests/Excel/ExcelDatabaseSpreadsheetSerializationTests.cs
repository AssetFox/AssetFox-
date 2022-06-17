﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Excel
{
    public class ExcelDatabaseSpreadsheetSerializationTests
    {
        [Fact]
        public void DatabaseSpreadsheet_SerializeThenDeserialize_Equivalent()
        {
            var data1 = new List<IExcelCellDatum>
            {
                TestExcelCellData.HelloDatum(),
                TestExcelCellData.HelloCommaWorldDatum(),
                TestExcelCellData.PiDatum(),
                TestExcelCellData.PunctuationDatum(),
            };

            var now = DateTime.Now;

            var data2 = new List<IExcelCellDatum>
            {
                ExcelCellData.DateTime(now),
                TestExcelCellData.PiDatum(),
                ExcelCellData.Double(16),
            };
            var column1 = ExcelDatabaseColumns.WithEntries(data1);
            var column2 = ExcelDatabaseColumns.WithEntries(data2);
            var worksheet = ExcelDatabaseWorksheets.WithColumns(column1, column2);
            var serialized = ExcelDatabaseWorksheetSerializer.Serialize(worksheet);
            var deserialized = ExcelDatabaseWorksheetSerializer.Deserialize(serialized);
            var roundtrippedWorksheet = deserialized.Worksheet;
            ObjectAssertions.Equivalent(worksheet, roundtrippedWorksheet);
            // Equivalent is not quite enough. We also want to know that the types of the cells are preserved.
            var types2 = column2.Entries.Select(e => e.GetType().Name).ToList();
            var roundtrippedTypes2 = roundtrippedWorksheet.Columns[1].Entries.Select(e => e.GetType().Name).ToList();
            ObjectAssertions.Equivalent(types2, roundtrippedTypes2);
        }
    }
}
