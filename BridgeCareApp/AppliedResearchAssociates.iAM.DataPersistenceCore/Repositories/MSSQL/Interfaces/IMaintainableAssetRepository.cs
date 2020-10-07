using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IMaintainableAssetRepository
    {
        void AddNetworkMaintainableAssets(IEnumerable<MaintainableAsset> maintainableAssets, Guid networkId);
    }
}
