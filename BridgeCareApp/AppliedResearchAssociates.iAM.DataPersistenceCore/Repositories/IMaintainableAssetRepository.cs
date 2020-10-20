using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        IEnumerable<MaintainableAsset> GetAllInNetworkWithAssignedData(Guid networkId);
    }
}
