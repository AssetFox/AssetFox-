using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        List<Data.Networking.MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId);

        MaintainableAsset GetAssetAtLocation(Location location);

        void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId);

        void CreateMaintainableAssets(List<AnalysisMaintainableAsset> maintainableAssets, Guid networkId);

        void UpdateMaintainableAssetsSpatialWeighting(List<Data.Networking.MaintainableAsset> maintainableAssets);
    }
}
