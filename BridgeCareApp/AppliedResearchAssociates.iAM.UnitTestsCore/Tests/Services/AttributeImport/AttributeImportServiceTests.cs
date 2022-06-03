using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class AttributeImportServiceTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;
        private const string SampleAttributeDataFilename = "SampleAttributeData.xlsx";
        private AttributeImportService CreateAttributeImportService()
        {
            var returnValue = new AttributeImportService(_testHelper.UnitOfWork);
            return returnValue;
        }

        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilename;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, "SampleData", filename);
            return returnValue;
        }

        private Stream FileContent(string path)
        {
            var file = File.OpenRead(path);
            return file;
        }

        [Fact]
        public void ImportSpreadsheet_IdKeyIsNotColumnTitle_FailsWithWarning()
        {
            var path = SampleAttributeDataPath();
            var stream = FileContent(path);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var excelPackage = new ExcelPackage(memoryStream);
            var service = CreateAttributeImportService();
            var result = service.ImportExcelAttributes("nonExistentColumn", excelPackage);
            var warningMessage = result.WarningMessage;
            Assert.Contains(AttributeImportService.NoColumnFoundForId, warningMessage);
        }
    }
}
