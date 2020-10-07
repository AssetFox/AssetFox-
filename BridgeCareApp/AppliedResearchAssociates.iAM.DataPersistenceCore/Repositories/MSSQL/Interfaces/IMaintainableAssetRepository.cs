using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IMaintainableAssetRepository
    {
        void AddNetworkSegments(IEnumerable<MaintainableAsset> maintainableAssets, Guid networkId);
    }
}
