using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Models;
using BridgeCareCoreTests.Helpers;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;

namespace BridgeCareCoreTests.Tests.Integration
{
    public static class NetworkIntegrationTestSetup
    {
        public static TNetwork ModelForEntityInDbViaFactory(IUnitOfWork unitOfWork, DataAttribute attribute, NetworkCreationParameters parameters, string networkName)
        {
            var network = NetworkTestSetupViaFactory.ModelViaFactory(unitOfWork, attribute, parameters, networkName);

            // insert network domain data into the data source
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }
    }
}
