using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        int UpdateAssignedData(Network network);
    }
}
