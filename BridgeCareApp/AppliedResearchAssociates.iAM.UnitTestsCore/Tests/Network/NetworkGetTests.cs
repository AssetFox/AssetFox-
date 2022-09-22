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
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkGetTests
    {

        private TestHelper _testHelper => TestHelper.Instance;

        [Fact]
        public void GetSimulationAnalysisNetwork_NetworkInDbCreatedViaFactory_Does()
        {
            var config = _testHelper.Config;
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(_testHelper.UnitOfWork, connectionString);
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
            var network = NetworkTestSetup.ModelForEntityInDbViaFactory(
                _testHelper.UnitOfWork, districtAttributeDomain, parameters, networkName);
            var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();

            var simulationAnalysisNetwork = _testHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(network.Id, explorer);

            Assert.Equal(network.Id, simulationAnalysisNetwork.Id);
        }


        
        public void RunGetSimulationAnalysisNetwork_NetworkInDb_Does(int assetCount)
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
            var network = NetworkTestSetup.ModelForEntityInDb(_testHelper.UnitOfWork, maintainableAssets, networkId);
            var config = _testHelper.Config;
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(_testHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);
            var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();

            var simulationAnalysisNetwork = _testHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(network.Id, explorer);

            Assert.Equal(network.Id, simulationAnalysisNetwork.Id);
            var assets = simulationAnalysisNetwork.Assets;
            Assert.Equal(assetCount, assets.Count);
        }

        [Fact]
        public void GetSimulationAnalysisNetwork_NetworkInDb33000Assets_Does()
        {
            RunGetSimulationAnalysisNetwork_NetworkInDb_Does(33000);
        }
    }
}
