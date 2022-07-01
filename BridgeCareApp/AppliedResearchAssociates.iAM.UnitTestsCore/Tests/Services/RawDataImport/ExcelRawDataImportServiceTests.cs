using System;
using System.IO;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class ExcelRawDataImportServiceTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;
        public const string SpatialWeighting = "[DECK_AREA]";
        public const string BrKey = "BRKEY";
        public const string InspectionDateColumnTitle = "Inspection_Date";

        private static ExcelRawDataImportService CreateExcelSpreadsheetImportService()
        {
            var returnValue = TestServices.CreateExcelSpreadsheetImportService(_testHelper.UnitOfWork);
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
            _testHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);
            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);
            var spreadsheetId = importResult.RawDataId;
            var upsertedSpreadsheet = _testHelper.UnitOfWork.ExcelWorksheetRepository.GetExcelRawData(spreadsheetId);
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
            // WjTodo -- Eventually, we will need a family of tests for actually creating networks. Something like the commented-out code below will go into that family of tests. 6/21/2022
            //var result = attributeService.ImportExcelAttributes(/*"BRKEY", InspectionDateColumnTitle, SpatialWeighting, these fields should be in the dataSource object*/ spreadsheetId);
            //var warningMessage = result.WarningMessage;
            //Assert.True(string.IsNullOrEmpty(warningMessage));
            //var networkId = result.NetworkId.Value;
            //var assets = _testHelper.UnitOfWork.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId);
            //var assetCount = assets.Count;
            //Assert.Equal(4, assetCount);
            //var datum0 = assets[0].AssignedData[0];
            //var stringDatum0 = datum0 as AttributeDatum<string>;
            //Assert.NotNull(stringDatum0);
            //Assert.NotNull(stringDatum0.Value);
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
            _testHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);

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
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var importResult = spreadsheetService.ImportRawData(dataSourceId, excelPackage.Workbook.Worksheets[0]);
            var warning = importResult.WarningMessage;
        }
    }
}
