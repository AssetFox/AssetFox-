using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class NetworkTestSetup
    {
        public static TNetwork ModelForEntityInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, Guid? networkId = null)
        {
            var resolvedNetworkId = networkId ?? Guid.NewGuid();
            var network = new TNetwork(maintainableAssets, resolvedNetworkId);
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }
    }
}
