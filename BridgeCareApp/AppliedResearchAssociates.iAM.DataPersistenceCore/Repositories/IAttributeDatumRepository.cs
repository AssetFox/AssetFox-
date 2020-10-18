using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        int UpdateAssignedData(Network network);
        IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId);
    }
}
