using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Aggregation;
using OfficeOpenXml;
using Xunit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using BridgeCareCoreTests.Helpers;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class SqlAggregationTests
    {
        [Fact (Skip ="WJ is skeptical that he can remove the database from this one")]
        public async Task Aggregate_SqlDataSourceInDb_AttributesInDb_Aggregates()
        {
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(TestHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);

            var networkName = RandomStrings.WithPrefix("Network");
            var allDataSourceDto = AllDataSourceDtoFakeFrontEndFactory.ToAll(dataSourceDto);

            var networkDefinitionAttribute = AllAttributeDtos.BrKey(allDataSourceDto);
            var parameters = new NetworkCreationParameters
            {
                DefaultEquation = "[Deck_Area]",
                NetworkDefinitionAttribute = networkDefinitionAttribute
            };
            var network = NetworkIntegrationTestSetup.ModelForEntityInDbViaFactory(
                TestHelper.UnitOfWork, districtAttributeDomain, parameters, networkName);

            var networkId = network.Id;
            var assetName = "AssetName";
            var location = new SectionLocation(Guid.NewGuid(), assetName);
            var maintainableAssetId = Guid.NewGuid();
            var spatialWeightingValue = "[Deck_Area]";
            var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue);
            var assetList = new List<MaintainableAsset> { newAsset };
            TestHelper.UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(assetList, networkId);

            var aggregationService = new AggregationService(TestHelper.UnitOfWork);

            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var aggregationState = new AggregationState();
            var attributes = new List<AttributeDTO> { districtAttribute };
            var aggregationResult = await aggregationService.AggregateNetworkData(channel.Writer, networkId, aggregationState, attributes);
            Assert.True(aggregationResult);
        }
    }
}
