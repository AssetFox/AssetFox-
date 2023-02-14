using System;
using System.IO;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
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

        private static ExcelRawDataImportService CreateExcelSpreadsheetImportService(Mock<IUnitOfWork> unitOfWork = null)
        {
            var service = new ExcelRawDataImportService(unitOfWork.Object);
            return service;
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
            var unitOfWork = UnitOfWorkMocks.New();
            var dataSourceRepo = DataSourceRepositoryMocks.New(unitOfWork);
            var rawDataRepo = ExcelRawDataRepositoryMocks.New(unitOfWork);
            var spreadsheetService = CreateExcelSpreadsheetImportService(unitOfWork);
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
            var unitOfWork = UnitOfWorkMocks.New();
            var dataSourceRepo = DataSourceRepositoryMocks.New(unitOfWork);
            var rawDataRepo = ExcelRawDataRepositoryMocks.New(unitOfWork);
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            dataSourceRepo.Setup(d => d.GetDataSource(dataSourceId)).Returns(dataSourceDto);
            var service = CreateExcelSpreadsheetImportService(unitOfWork);

            var result = service.ImportRawData(dataSourceId, excelPackage.Workbook.Worksheets[0]);

            var warningMessage = result.WarningMessage;
            Assert.Equal(warningMessage, ExcelRawDataImportService.TopSpreadsheetRowIsEmpty);
        }


        [Fact]
        public void ImportRawDataSpreadsheet_DataSourceDoesNotExist_FailsWithWarning()
        {
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var unitOfWork = UnitOfWorkMocks.New();
            var dataSourceRepo = DataSourceRepositoryMocks.New(unitOfWork);
            var spreadsheetService = CreateExcelSpreadsheetImportService(unitOfWork);
            var dataSourceId = Guid.NewGuid();

            var importResult = spreadsheetService.ImportRawData(dataSourceId, excelPackage.Workbook.Worksheets[0]);

            var warning = importResult.WarningMessage;
            Assert.Contains(ExcelRawDataImportService.DataSourceDoesNotExist, warning);
        }

        [Fact]
        public void ImportSpreadsheet_ColumnNames_Expected()
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
