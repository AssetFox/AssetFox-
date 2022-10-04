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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkCreationTests
    {
        [Fact]
        public void CreateNetworkViaFactoryAndRepository_Does()
        {
            var networkName = RandomStrings.WithPrefix("Network");
            var config = TestConfiguration.Get();
            var allNetworksBefore = TestHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
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

            var allNetworks = TestHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
            var actual = allNetworks.Single(n => n.Id == networkId);
        }
    }
}
