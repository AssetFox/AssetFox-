using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        List<Data.Networking.MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId);

        void CreateMaintainableAssets(List<Data.Networking.MaintainableAsset> maintainableAssets, Guid networkId);

        void CreateMaintainableAssets(List<Analysis.MaintainableAsset> maintainableAssets, Guid networkId);

        void UpdateMaintainableAssetsSpatialWeighting(List<Data.Networking.MaintainableAsset> maintainableAssets);
    }
}
