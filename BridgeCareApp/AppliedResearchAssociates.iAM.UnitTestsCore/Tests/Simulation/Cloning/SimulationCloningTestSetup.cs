using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationCloning
{
    public static class SimulationCloningTestSetup
    {
        public static Guid TestNetworkIdInDatabase()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            return networkId;
        }

        public static Guid TestDestinationNetworkIdInDatabase()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var maintainableAssets = new List<MaintainableAsset>();
            var destinationNetworkId = Guid.NewGuid();
            NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, TestAttributeIds.BrKeyId, destinationNetworkId);
            return destinationNetworkId;
        }

    }
}
