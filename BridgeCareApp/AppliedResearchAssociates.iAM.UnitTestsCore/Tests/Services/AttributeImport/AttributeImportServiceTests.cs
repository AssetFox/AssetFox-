using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class AttributeImportServiceTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;
        public const string SpatialWeighting = "[DECK_AREA]";
        public const string BrKey = "BRKEY";
        public const string InspectionDateColumnTitle = "Inspection_Date";
        private AttributeImportService CreateAttributeImportService()
        {
            var returnValue = new AttributeImportService(_testHelper.UnitOfWork);
            return returnValue;
        }

        private ExcelSpreadsheetImportService CreateExcelSpreadsheetImportService()
        {
            var returnValue = new ExcelSpreadsheetImportService(_testHelper.UnitOfWork);
            return returnValue;
        }


        private const string DistrictAttributeName = "DISTRICT";
        private const string SuffRateAttributeName = "SUFF_RATE";

        private static void EnsureAttributeExists(AttributeDTO dto)
        {
            var existingDistrictAttribute = _testHelper.UnitOfWork.AttributeRepo.GetSingleByName(dto.Name);
            if (existingDistrictAttribute == null)
            {
                _testHelper.UnitOfWork.AttributeRepo.UpsertAttributes(dto);
            }
        }


        private static AttributeDTO DistrictAttributeDto() => new AttributeDTO
        {
            Name = DistrictAttributeName,
            AggregationRuleType = TextAttributeAggregationRules.Predominant,
            Id = Guid.NewGuid(),
            Command = "DistrictCommand",
            DefaultValue = "Default District",
            Type = DataPersistenceConstants.AttributeTextDataType,
            IsAscending = false,
            IsCalculated = false,
        };

        private static void EnsureSuffRateAttributeExists()
        {
            var dto = SuffRateAttributeDto();
            EnsureAttributeExists(dto);
        }

        private static void EnsureDistrictAttributeExists()
        {
            var dto = DistrictAttributeDto();
            EnsureAttributeExists(dto);
        }


        private static AttributeDTO SuffRateAttributeDto() => new AttributeDTO
        {
            Name = SuffRateAttributeName,
            AggregationRuleType = NumericAttributeAggregationRules.Average,
            Id = Guid.NewGuid(),
            Command = "SuffRateCommand",
            DefaultValue = "77",
            Minimum = 0,
            Maximum = 100,
            Type = DataPersistenceConstants.AttributeNumericDataType,
            IsAscending = false,
            IsCalculated = false,
        };

        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, "SampleData", filename);
            return returnValue;
        }

        private static string SampleAttributeDataPathWithSuffRatePath()
        {
            var filename = SampleAttributeDataFilenames.WithSuffRate;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, "SampleData", filename);
            return returnValue;
        }
        private static string SampleAttributeDataWithSpuriousEmptyFirstRowPath()
        {
            var filename = SampleAttributeDataFilenames.WithSpuriousEmptyFirstRow;
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
        public void ImportSpreadsheet_ColumnHeaderIsAttributeName_CreatesNetworkAndAttributes()
        {
            _testHelper.CreateAttributes();
            EnsureDistrictAttributeExists();
            var path = SampleAttributeDataPath();
            var stream = FileContent(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = CreateExcelSpreadsheetImportService();
            var attributeService = CreateAttributeImportService();
            var dataSourceId = Guid.NewGuid();
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = "DataSourceName",
            };
             _testHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);
            var spreadsheetId = spreadsheetService.ImportSpreadsheet(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);
            var upsertedSpreadsheet = _testHelper.UnitOfWork.ExcelWorksheetRepository.GetExcelWorksheet(spreadsheetId);
            var expectedUpsertedSpreadsheet = new ExcelSpreadsheetDTO
            {
                Id = spreadsheetId,
                DataSourceId = dataSourceId,
                SerializedWorksheetContent = upsertedSpreadsheet.SerializedWorksheetContent,
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

        //[Fact]
        //public void ImportSpreadsheet_KeyColumnNameIsEmpty_FailsWithWarning()
        //{
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var service = CreateAttributeImportService();
        //    var importResult = service.ImportExcelAttributes("", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warning = importResult.WarningMessage;
        //    Assert.Equal(AttributeImportService.NonemptyKeyIsRequired, warning);
        //}

        //[Fact]
        //public void ImportSpreadsheet_IdKeyIsNotColumnTitle_FailsWithWarning()
        //{
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("nonExistentColumn", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.NoColumnFoundForId, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_InspectionDateColumnTitleIsNotColumnTitle_FailsWithWarning()
        //{
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes(BrKey, "NonexistentColumn", SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.InspectionDateColumn, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNotAttributeName_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var randomString = RandomStrings.Length11();
        //    excelPackage.Workbook.Worksheets[0].Cells[1, 4].Value = randomString;
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.NoAttributeWasFoundWithName, warningMessage);
        //}


        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNameOfDoubleAttribute_CreatesNetworkAndAttributes()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    EnsureSuffRateAttributeExists();
        //    var path = SampleAttributeDataPathWithSuffRatePath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.True(string.IsNullOrEmpty(warningMessage));
        //    var networkId = result.NetworkId.Value;
        //    var assets = _testHelper.UnitOfWork.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId);
        //    var assetCount = assets.Count;
        //    var datum0 = assets[0].AssignedData[0];
        //    var doubleDatum0 = datum0 as AttributeDatum<double>;
        //    Assert.NotNull(doubleDatum0);
        //    Assert.Equal(4, assetCount);
        //}

        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNameOfDoubleAttributeButOneEntryIsAlphabetical_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    EnsureSuffRateAttributeExists();
        //    var path = SampleAttributeDataPathWithSuffRatePath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[3, 3].Value = "This is not a number";
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.FailedToCreateAValidAttributeDatum, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNameOfDoubleAttributeButOneEntryIsTooBig_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    EnsureSuffRateAttributeExists();
        //    var path = SampleAttributeDataPathWithSuffRatePath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[2, 3].Value = 1000;
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.IsNotLessThanOrEqualToTheMaximumValue, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNameOfDoubleAttributeButOneEntryIsTooSmall_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    EnsureSuffRateAttributeExists();
        //    var path = SampleAttributeDataPathWithSuffRatePath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[2, 3].Value = -2;
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.IsNotGreaterThanOrEqualToTheMinimumValue, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_ColumnHeaderIsNameOfDoubleAttributeButOneValueIsEmpty_UsesDefaultValueForThatAttributeDatum()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    EnsureSuffRateAttributeExists();
        //    var path = SampleAttributeDataPathWithSuffRatePath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[2, 3].Value = "";
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var networkId = result.NetworkId.Value;
        //    var assets = _testHelper.UnitOfWork.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId);
        //    var asset1 = assets.Single(a => a.Location.LocationIdentifier == "1");
        //    var asset1AttributeDatum = asset1.AssignedData[0];
        //    var doubleAsset1AttributeDatum = (AttributeDatum<double>)asset1AttributeDatum;
        //    var value = doubleAsset1AttributeDatum.Value;
        //    Assert.Equal(77, value);
        //}

        //[Fact]
        //public void ImportSpreadsheet_RepeatedRowKey_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[3, 1].Value = worksheet.Cells[2, 1].Value;
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Contains(AttributeImportService.WasFoundInRow, warningMessage);
        //}

        //[Fact]
        //public void ImportSpreadsheet_TopLineIsBlank_FailsWithWarning()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    var path = SampleAttributeDataWithSpuriousEmptyFirstRowPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.Equal(warningMessage, AttributeImportService.TopSpreadsheetRowIsEmpty);
        //}

        //public void ImportSpreadsheet_EmptyDatum_AddsAttributeWithDefaultValue()
        //{
        //    _testHelper.CreateAttributes();
        //    EnsureDistrictAttributeExists();
        //    var path = SampleAttributeDataPath();
        //    var stream = FileContent(path);
        //    var excelPackage = new ExcelPackage(stream);
        //    var worksheet = excelPackage.Workbook.Worksheets[0];
        //    worksheet.Cells[2, 2].Value = "";
        //    var service = CreateAttributeImportService();
        //    var result = service.ImportExcelAttributes("BRKEY", InspectionDateColumnTitle, SpatialWeighting, excelPackage);
        //    var warningMessage = result.WarningMessage;
        //    Assert.True(string.IsNullOrEmpty(warningMessage));
        //    var networkId = result.NetworkId.Value;
        //    var assets = _testHelper.UnitOfWork.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId);
        //    var assetCount = assets.Count;
        //    Assert.Equal(4, assetCount);
        //    var assetNamed1 = assets.Single(a => a.Location.LocationIdentifier == "1");
        //    var datum0 = assetNamed1.AssignedData[0];
        //    var stringDatum0 = datum0 as AttributeDatum<string>;
        //    Assert.NotNull(stringDatum0);
        //    Assert.Equal("Default District", stringDatum0.Value);
        //}
    }
}
