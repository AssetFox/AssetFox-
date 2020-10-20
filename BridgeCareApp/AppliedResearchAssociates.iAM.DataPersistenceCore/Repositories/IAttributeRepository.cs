using System;
using System.Collections.Generic;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeRepository
    {
        Dictionary<Guid, Attribute> GetAttributeDictionary(string filePath);
        void CreateMissingAttributes(List<Attribute> attributes);
        IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId);
    }
}
