using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        void AddAssignedData(List<MaintainableAsset> maintainableAssets);
    }
}
