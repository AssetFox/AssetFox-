using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

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

        private void EnsureDistrictAttributeExists()
        {
            var existingDistrictAttribute = _testHelper.UnitOfWork.AttributeRepo.GetSingleByName("DISTRICT");
            if (existingDistrictAttribute == null)
            {
                var dto = new AttributeDTO
                {
                    Name = "DISTRICT",
                    AggregationRuleType = TextAttributeAggregationRules.Predominant,
                    Id = Guid.NewGuid(),
                    Command = "DistrictCommand",
                    DefaultValue = "",
                    Type = DataPersistenceConstants.AttributeTextDataType,
                    IsAscending = false,
                    IsCalculated = false,
                };
                _testHelper.UnitOfWork.AttributeRepo.UpsertAttributes(dto);
            }
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
        [Fact]
        public void ImportSpreadsheet_ColumnHeaderIsNotAttributeName_FailsWithWarning()
        {
            _testHelper.CreateAttributes();
            var path = SampleAttributeDataPath();
            var stream = FileContent(path);
            var excelPackage = new ExcelPackage(stream);
            var randomString = RandomStrings.Length11();
            excelPackage.Workbook.Worksheets[0].Cells[1, 2].Value = randomString;
            var service = CreateAttributeImportService();
            var result = service.ImportExcelAttributes("BRKEY", excelPackage);
            var warningMessage = result.WarningMessage;
            Assert.Contains(AttributeImportService.NoAttributeWasFoundWithName, warningMessage);
        }

        [Fact]
        public void ImportSpreadsheet_ColumnHeaderIsAttributeName_IdkWhat()
        {
            _testHelper.CreateAttributes();
            EnsureDistrictAttributeExists();
            var path = SampleAttributeDataPath();
            var stream = FileContent(path);
            var excelPackage = new ExcelPackage(stream);
            var service = CreateAttributeImportService();
            var result = service.ImportExcelAttributes("BRKEY", excelPackage);
            var warningMessage = result.WarningMessage;
            Assert.Contains(AttributeImportService.NoAttributeWasFoundWithName, warningMessage);
        }
    }
}
