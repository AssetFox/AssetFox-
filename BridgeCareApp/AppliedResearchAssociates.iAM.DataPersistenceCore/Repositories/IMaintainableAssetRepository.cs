using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository : IRepository<MaintainableAsset>
    {
        IEnumerable<MaintainableAsset> GetAllInNetwork(string networkId);
    }
}
