using System;
using System.Collections.Generic;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeRepository
    {
        void CreateAttributes(IEnumerable<Attribute> attributes);
        IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId);
    }
}
