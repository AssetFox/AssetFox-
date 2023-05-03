using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        void AddAssignedData(List<MaintainableAsset> maintainableAssets, List<AttributeDTO> attributeDtos);

        List<AttributeDatumDTO> GetAllInNetwork(IEnumerable<Guid> networkMaintainableAssetIds, List<Guid> requiredAttributeIds);
    }

    public interface IExtendedAttributeDatumRepository : IAttributeDatumRepository
    {
        List<AttributeDatumDTO> GetRawData(Guid networkId, Dictionary<AttributeDTO, string> dictionary);
        //void GetRawData(Guid networkId, HashSet<AttributeDTO> attributeDtos);
    }

}
