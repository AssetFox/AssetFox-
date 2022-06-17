using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            // We can do better . . .
            //[["\u0022BRKEY\u0022","1","10","100","10001"],["\u0022DISTRICT\u0022","11","20","110","10011"],["\u0022Inspection_Date\u0022","D\u00222022-01-01T00:00:00\u0022","D\u00222022-02-01T00:00:00\u0022","D\u00222022-03-01T00:00:00\u0022","D\u00222022-04-01T00:00:00\u0022"]]
            var entity = _testHelper.UnitOfWork.Context.ExcelWorksheets.Single(w => w.Id == result);
            var serializedContent = entity.SerializedWorksheetContent;
        }

        [Fact]
        public void NestedObjectListSerializationTest()
        {
            var objects = new List<List<object>>
            {
                new List<object>
                {
                    1,
                    "hello"
                },
                new List<object>
                {
                    new DateTime(2020, 3, 1),
                }
            };
            var serializedObjects = JsonSerializer.Serialize(objects);
            var deserializedObjects = JsonSerializer.Deserialize<List<List<object>>>(serializedObjects);
        }
    }
}
