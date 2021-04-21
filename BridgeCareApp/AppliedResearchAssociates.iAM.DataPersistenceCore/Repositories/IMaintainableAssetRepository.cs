﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IMaintainableAssetRepository
    {
        List<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId);

        void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId);

        void CreateMaintainableAssets(List<Section> sections, Guid networkId);

        void UpdateMaintainableAssetsSpatialWeighting(List<MaintainableAsset> maintainableAssets);

        List<BMSIDAndBRKeyDTO> GetBMSIDAndBRKey();
    }
}
