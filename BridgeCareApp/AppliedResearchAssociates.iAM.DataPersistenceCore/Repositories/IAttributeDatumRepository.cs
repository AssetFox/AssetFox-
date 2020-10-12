using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        bool DeleteAssignedDataFromNetwork(Guid networkId);
        void AddAttributeData(IEnumerable<IAttributeDatum> domainAttributeData, Guid maintainableAssetId);
    }
}
