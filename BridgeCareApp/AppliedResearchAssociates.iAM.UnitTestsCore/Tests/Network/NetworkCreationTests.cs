using System;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataUnitTests;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkCreationTests
    {
        [Fact]
        public async Task CreateNetworkViaFactoryAndRepository_Does()
        {
            var networkName = RandomStrings.WithPrefix("Network");
            var config = TestConfiguration.Get();
            var allNetworksBefore = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(TestHelper.UnitOfWork, connectionString);
            var attribute = UnitTestsCoreAttributeTestSetup.ExcelAttributeForEntityInDb(dataSourceDto);
            var defaultEquation = "[Deck_Area]";
            var textAttribute = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var attributeConnection = new SqlAttributeConnection(textAttribute, dataSourceDto);
            // var attributeConnection = AttributeConnectionBuilder.Build(textAttribute, dataSourceDto, TestHelper.UnitOfWork);
            var data = attributeConnection.GetData<string>();
            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                  data, defaultEquation);
            network.Name = networkName;
            var networkId = network.Id;

            // insert network domain data into the data source
            TestHelper.UnitOfWork.NetworkRepo.CreateNetwork(network);

            var networkIds = new List<Guid> { networkId };
            var allNetworks = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var actual = allNetworks.Single(n => n.Id == networkId);
            Assert.Equal(allNetworksBefore.Count + 1, allNetworks.Count);
            Assert.Equal(networkName, actual.Name);
        }
    }
}
