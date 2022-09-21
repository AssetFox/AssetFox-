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
    public class NetworkCreationTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        [Fact]
        public void CreateNetworkViaFactoryAndRepository_Does()
        {
            var networkName = RandomStrings.WithPrefix("Network");
            var config = _testHelper.Config;
            var allNetworksBefore = _testHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(_testHelper.UnitOfWork, connectionString);
            var attribute = UnitTestsCoreAttributeTestSetup.ExcelAttributeForEntityInDb(dataSourceDto);
            var allDataSourceDto = AllDataSourceDtoFakeFrontEndFactory.ToAll(dataSourceDto);

            var networkDefinitionAttribute = AllAttributeDtos.BrKey(allDataSourceDto);
            var parameters = new NetworkCreationParameters
            {
                DefaultEquation = "[Deck_Area]",
                NetworkDefinitionAttribute = networkDefinitionAttribute
            };
            var allDataSource = parameters.NetworkDefinitionAttribute.DataSource;
            var mappedDataSource = AllDataSourceMapper.ToSpecificDto(allDataSource);
            var textAttribute = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var attributeConnection = AttributeConnectionBuilder.Build(textAttribute, mappedDataSource, _testHelper.UnitOfWork);
            var data = AttributeDataBuilder.GetData(attributeConnection);
            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                  data, parameters.DefaultEquation);
            network.Name = networkName;
            var networkId = network.Id;

            // insert network domain data into the data source
            _testHelper.UnitOfWork.NetworkRepo.CreateNetwork(network);

            var allNetworks = _testHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
            var actual = allNetworks.Single(n => n.Id == networkId);
        }

        [Fact]
        public void GetSimulationAnalysisNetwork_NetworkInDb_Does()
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
    }
}
