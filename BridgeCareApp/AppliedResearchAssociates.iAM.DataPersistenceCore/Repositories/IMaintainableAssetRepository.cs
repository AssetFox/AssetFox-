﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        List<Data.Networking.MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId);
        bool CheckIfKeyAttributeValueExists(Guid networkId, string attributeValue);

        [Obsolete("Not actually obsolete. But not currently under test and probably broken. Test before using. But Jake says that can't be done until new Network code is in place, which it is not as of June 20, 2022.")]
        MaintainableAsset GetAssetAtLocation(Location location);

        void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId);

        void CreateMaintainableAssets(List<AnalysisMaintainableAsset> maintainableAssets, Guid networkId);

        void UpdateMaintainableAssetsSpatialWeighting(List<Data.Networking.MaintainableAsset> maintainableAssets);
        MaintainableAsset GetMaintainableAssetByKeyAttribute(Guid networkId, string attributeValue);
    }
}
