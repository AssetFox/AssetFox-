using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkRepositoryTests
    {
        [Fact]
        public void GetNetworkNameOrId_NetworkNotInDb_GetsId()
        {
            var networkId = Guid.NewGuid();
            var networkNameOrId = TestHelper.UnitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
            Assert.Contains(networkNameOrId.ToString(), networkNameOrId);
        }

        [Fact (Skip ="Test found an issue which needs fixing. Should not be skipped at PR time.")]
        public async Task DeleteNetwork_NetworkInDb_Deletes()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var attributeDto = AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, "location");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId);
            var networksBefore = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var networkBefore = networksBefore.Single(n => n.Id == networkId);

            TestHelper.UnitOfWork.NetworkRepo.DeleteNetwork(networkId);

            var networksAfter = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var networkAfter = networksAfter.SingleOrDefault(n => n.Id == networkId);
            Assert.Null(networkAfter);
        }

        [Fact(Skip = "Should not be skipped at PR time.")]
        public async Task DeleteNetwork_NetworkInDbWithSimulation_DeletesBoth()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var attributeDto = AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, "location");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(
                TestHelper.UnitOfWork, simulationId, "simulation should be deleted", user.Id, networkId);
            var networksBefore = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var networkBefore = networksBefore.Single(n => n.Id == networkId);
            var simulationEntityBefore = TestHelper.UnitOfWork.Context.Simulation.SingleOrDefault(
               s => s.Id == simulationId);
            Assert.NotNull(simulationEntityBefore);

            TestHelper.UnitOfWork.NetworkRepo.DeleteNetwork(networkId);

            var networksAfter = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var networkAfter = networksAfter.SingleOrDefault(n => n.Id == networkId);
            Assert.Null(networkAfter);
            var simulationEntityAfter = TestHelper.UnitOfWork.Context.Simulation.SingleOrDefault(
                s => s.Id == simulationId);
            Assert.Null(simulationEntityAfter);
        }

        [Fact]
        public void GetNetworkName_NetworkInDb_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId, "Network name");

            var name = TestHelper.UnitOfWork.NetworkRepo.GetNetworkName(networkId);

            Assert.Equal("Network name", name);
        }

        [Fact]

        public async Task UpsertNetworkRollupDetail_NetworkInDb_Does()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;

            TestHelper.UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, "Test rollup status");

            var networksAfter = await TestHelper.UnitOfWork.NetworkRepo.Networks();
            var networkAfter = networksAfter.Single(n => n.Id == networkId);
            Assert.Equal("Test rollup status", networkAfter.Status);
        }

        [Fact]
        public void GetNetworkKeyAttribute_NetworkInDb_GetsAttributeName()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;

            var keyAttribute = TestHelper.UnitOfWork.NetworkRepo.GetNetworkKeyAttribute(networkId);

            Assert.Equal("BRKEY_", keyAttribute);
        }

        [Fact]
        public async Task GetRawNetwork_RawNetworkHasBeenSet_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var networkName = RandomStrings.WithPrefix("Network name");
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId, networkName);
            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkName);

            var rawNetwork = TestHelper.UnitOfWork.NetworkRepo.GetRawNetwork();

            Assert.Equal(networkName, rawNetwork.Name);
            Assert.Equal(networkId, rawNetwork.Id);
        }
    }
}
