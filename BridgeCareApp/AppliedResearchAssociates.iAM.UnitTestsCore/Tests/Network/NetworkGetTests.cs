using System;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Mappers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkGetTests
    {

        private void RunGetSimulationAnalysisNetwork_NetworkInDb_Does(int assetCount, int aggregatedResultPerAssetCount)
        {
            var networkId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            for (int i = 0; i < assetCount; i++)
            {
                var assetId = Guid.NewGuid();
                var locationIdentifier = RandomStrings.WithPrefix("Location");
                var location = Locations.Section(locationIdentifier);
                var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
                maintainableAssets.Add(maintainableAsset);
            }
            var resultAttributes = new List<IamAttribute>();
            for (int i = 0; i < aggregatedResultPerAssetCount; i++)
            {
                var resultAttribute = AttributeTestSetup.Numeric();
                resultAttributes.Add(resultAttribute);
            }
            TestHelper.UnitOfWork.AttributeRepo.UpsertAttributesNonAtomic(resultAttributes);
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId);
            AggregatedResultTestSetup.AddNumericAggregatedResultsToDb(TestHelper.UnitOfWork, maintainableAssets, resultAttributes);

            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(TestHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeDtoDomainMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);
            var explorer = TestHelper.UnitOfWork.AttributeRepo.GetExplorer();

            var simulationAnalysisNetwork = TestHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(network.Id, explorer);

            Assert.Equal(network.Id, simulationAnalysisNetwork.Id);
            var assets = simulationAnalysisNetwork.Assets;
            Assert.Equal(assetCount, assets.Count);
            foreach (var asset in assets)
            {
                var historicalAttributeList = asset.HistoricalAttributes.ToList();
                Assert.Equal(aggregatedResultPerAssetCount, historicalAttributeList.Count);
            }
        }


        [Fact]
        public void GetSimulationAnalysisNetwork_NetworkInDb300Assets_Does()
        {
            RunGetSimulationAnalysisNetwork_NetworkInDb_Does(300, 10);
        }
    }
}
