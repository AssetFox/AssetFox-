using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeDatumRepository
    {
        void AddAttributeData(IEnumerable<IAttributeDatum> domainAttributeData, Guid maintainableAssetId);
    }
}
