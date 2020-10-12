using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        void AddNetworkMaintainableAssets(IEnumerable<MaintainableAsset> maintainableAssets, Guid networkId);

        IEnumerable<MaintainableAsset> GetMaintainableAssetsWithAssignedData(Guid networkId);
    }
}
