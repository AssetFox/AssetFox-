using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        int UpdateAssignedDataByAttributeId(Guid network, IEnumerable<Guid> assignedDataAttributeIdsToClear, IEnumerable<MaintainableAsset> maintainableAssets);
    }
}
