using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppliedResearchAssociates.BridgeCareCore.Testing
{
    public class NetworkControllerShould
    {
        [Fact]
        public void CreateNetwork()
        {
            var moqNetworkRepo = new Mock<INetworkRepository>();
            var moqAttributeMetaDataRepo = new Mock<IAttributeMetaDataRepository>();
            var moqLogger = new Mock<ILogger<NetworkController>>();

            var sectionLocation = new SectionLocation(Guid.Empty, "SampleIdentifier");
            var maintainableAsset = new MaintainableAsset(Guid.Empty, Guid.Empty, sectionLocation);
            var networkId = Guid.NewGuid();
            var network = new Network(new List<MaintainableAsset> { maintainableAsset }, networkId, "Sample Network");
            moqNetworkRepo.Setup(r => r.CreateNetwork(network)).Verifiable();

            moqAttributeMetaDataRepo.Setup(a => a.GetAllAttributes());
            moqAttributeMetaDataRepo.Setup(a => a.GetNetworkDefinitionAttribute());

            var controller = new NetworkController(moqAttributeMetaDataRepo.Object, moqNetworkRepo.Object, moqLogger.Object);

            var result = controller.CreateNetwork("Sample Network");

            Assert.Equal(result.ToString(), networkId.ToString());
        }
    }
}
