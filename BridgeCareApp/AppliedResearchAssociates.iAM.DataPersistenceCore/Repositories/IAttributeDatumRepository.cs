using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId);
        int UpdateAssignedData(Network network);
        int DeleteAssignedDataFromNetwork(Guid networkId);
    }
}
