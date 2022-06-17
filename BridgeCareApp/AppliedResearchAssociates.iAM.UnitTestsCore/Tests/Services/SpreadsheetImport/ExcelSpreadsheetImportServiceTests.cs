﻿using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services.SpreadsheetImport
{
    public class ExcelSpreadsheetImportServiceTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;
        private ExcelSpreadsheetImportService CreateExcelSpreadsheetImportService()
        {
            var returnValue = new ExcelSpreadsheetImportService(_testHelper.UnitOfWork);
            return returnValue;
        }
        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, "SampleData", filename);
            return returnValue;
        }

        [Fact]
        public void ImportSpreadsheet_InspectionDateColumnTitleIsNotColumnTitle_FailsWithWarning()
        {
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var service = CreateExcelSpreadsheetImportService();
            var worksheet = excelPackage.Workbook.Worksheets[0];
            var result = service.ImportSpreadsheet(worksheet);
            //var entity = _testHelper.UnitOfWork.Context.ExcelWorksheets.Single(w => w.Id == result);
            //var serializedContent = entity.SerializedWorksheetContent;
        }
    }
}
