using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        void AddAssignedData(List<MaintainableAsset> maintainableAssets, List<AttributeDTO> attributeDtos);
    }
}
