using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        int UpdateAssignedData(List<MaintainableAsset> maintainableAssets);
    }
}
