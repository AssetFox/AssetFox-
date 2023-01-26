using System;
using System.IO;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class ExcelRawDataImportServiceTests
    {
        public const string SpatialWeighting = "[DECK_AREA]";
        public const string BrKey = "BRKEY";
        public const string InspectionDateColumnTitle = "Inspection_Date";

        private static ExcelRawDataImportService CreateExcelSpreadsheetImportService()
        {
            var returnValue = TestServices.CreateExcelSpreadsheetImportService(TestHelper.UnitOfWork);
            return returnValue;
        }

        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, SampleAttributeDataFolderNames.SampleData, filename);
            return returnValue;
        }

        private static string SampleAttributeDataWithSpuriousEmptyFirstRowPath()
        {
            var filename = SampleAttributeDataFilenames.WithSpuriousEmptyFirstRow;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, SampleAttributeDataFolderNames.SampleData, filename);
            return returnValue;
        }

        [Fact]
        public void ImportRawData_DataSourceExists_Succeeds()
        {
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = CreateExcelSpreadsheetImportService();
            var dataSourceId = Guid.NewGuid();
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            TestHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);
            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);
            var spreadsheetId = importResult.RawDataId;
            var upsertedSpreadsheet = TestHelper.UnitOfWork.ExcelWorksheetRepository.GetExcelRawData(spreadsheetId);
            var serializedWorksheetContent = upsertedSpreadsheet.SerializedWorksheetContent;
            Assert.StartsWith("[[", serializedWorksheetContent);
            Assert.EndsWith("]]", serializedWorksheetContent);
            Assert.Contains("2022-03-01", serializedWorksheetContent);
            var expectedUpsertedSpreadsheet = new ExcelRawDataDTO
            {
                Id = spreadsheetId,
                DataSourceId = dataSourceId,
                SerializedWorksheetContent = serializedWorksheetContent,
            };
            ObjectAssertions.Equivalent(expectedUpsertedSpreadsheet, expectedUpsertedSpreadsheet);
        }

        [Fact]
        public void ImportRawDataSpreadsheet_TopLineIsBlank_FailsWithWarning()
        {
            var path = SampleAttributeDataWithSpuriousEmptyFirstRowPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var dataSourceId = Guid.NewGuid();
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            TestHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);

            var service = CreateExcelSpreadsheetImportService();
            var result = service.ImportRawData(dataSourceId, excelPackage.Workbook.Worksheets[0]);
            var warningMessage = result.WarningMessage;
            Assert.Equal(warningMessage, ExcelRawDataImportService.TopSpreadsheetRowIsEmpty);
        }


        [Fact]
        public void ImportRawDataSpreadsheet_DataSourceDoesNotExist_FailsWithWarning()
        {
            // When this test is run, the ExcelWorksheet entity appears in the database, with no corresponding DataSource entity. The foreign key is not enforced. And that needs fixing.
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = CreateExcelSpreadsheetImportService();
            var dataSourceId = Guid.NewGuid();
            var importResult = spreadsheetService.ImportRawData(dataSourceId, excelPackage.Workbook.Worksheets[0]);
            var warning = importResult.WarningMessage;
            Assert.Contains(ExcelRawDataImportService.DataSourceDoesNotExist, warning);
        }

        [Fact]
        public void RawDataSpreadsheetInDb_GetColumnNames_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var dataSourceRepo = DataSourceRepositoryMocks.New(unitOfWork);
            var rawDataRepo = ExcelRawDataRepositoryMocks.New(unitOfWork);
            rawDataRepo.Setup(r => r.AddExcelRawData(It.IsAny<ExcelRawDataDTO>())).Returns((ExcelRawDataDTO d) => d.Id);
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = TestServices.CreateExcelSpreadsheetImportService(unitOfWork.Object);
            var dataSourceId = Guid.NewGuid();
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            dataSourceRepo.Setup(d => d.GetDataSource(dataSourceId)).Returns(dataSourceDto);

            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);

            var spreadsheetId = importResult.RawDataId;
            var addCall = rawDataRepo.SingleInvocationWithName(nameof(IExcelRawDataRepository.AddExcelRawData));
            var upsertedSpreadsheet = addCall.Arguments[0] as ExcelRawDataDTO;
            var serializedWorksheetcontent = upsertedSpreadsheet.SerializedWorksheetContent;
            var deserializedWorksheetContent = ExcelRawDataSpreadsheetSerializer.Deserialize(serializedWorksheetcontent);
            var columnHeaders = deserializedWorksheetContent.Worksheet.Columns.Select(c => c.Entries.FirstOrDefault().ObjectValue().ToString()).ToList();
            var expectedColumnHeaders = new List<string> { "BRKEY", "DISTRICT", "Inspection_Date" };
            ObjectAssertions.Equivalent(expectedColumnHeaders, columnHeaders);
        }
    }
}
