using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Aggregation;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services.Aggregation
{
    public class ExcelAggregationTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, SampleAttributeDataFolderNames.SampleData, filename);
            return returnValue;
        }

        [Fact]
        public async Task Aggregate_ExcelDataSourceInDb_AttributesInDb_Aggregates()
        {
            var spreadsheetService = TestServices.CreateExcelSpreadsheetImportService(_testHelper.UnitOfWork);
            var dataSourceDto = DataSourceTestSetup.DtoForExcelDataSourceInDb(_testHelper.UnitOfWork);
            var districtAttribute = AttributeDtos.District(dataSourceDto);
            var districtAttributeDomain = AttributeMapper.ToDomain(districtAttribute);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);

            /// wjwjwj When I get back to this, the network creation should happen via NetworkFactory.CreateNetworkFromAttributeDataRecords().

            var networkName = RandomStrings.WithPrefix("Network");
            var attribute = UnitTestsCoreAttributeTestSetup.ExcelAttributeForEntityInDb(dataSourceDto);
            var allDataSourceDto = AllDataSourceDtoFakeFrontEndFactory.ToAll(dataSourceDto);

            var networkDefinitionAttribute = AllAttributeDtos.BrKey(allDataSourceDto);
            var parameters = new NetworkCreationParameters
            {
                DefaultEquation = "[Deck_Area]",
                NetworkDefinitionAttribute = networkDefinitionAttribute
            };
            var network = NetworkTestSetup.ModelForEntityInDbViaFactory(
                _testHelper.UnitOfWork, districtAttributeDomain, parameters, networkName);


            var networkId = network.Id;
            var assetName = "AssetName";
            var location = new SectionLocation(Guid.NewGuid(), assetName);
            var maintainableAssetId = Guid.NewGuid();
            var spatialWeightingValue = "[Deck_Area]";// wjwjwj this "[Deck_Area]" is wrong and will need to change
            var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue);
            var assetList = new List<MaintainableAsset> { newAsset };
            _testHelper.UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(assetList, networkId);


            var aggregationService = new AggregationService(_testHelper.UnitOfWork);

            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var aggregationState = new AggregationState();
            var attributes = new List<AttributeDTO> { districtAttribute };
            var aggregationResult = await aggregationService.AggregateNetworkData(channel.Writer, networkId, aggregationState, attributes);
            Assert.True(aggregationResult);
        }
    }
}
