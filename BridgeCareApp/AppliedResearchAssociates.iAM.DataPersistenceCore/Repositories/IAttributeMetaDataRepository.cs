using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<Attribute> GetAllAttributes();

        (Attribute Attribute, string DefaultEquation) GetNetworkDefinitionAttribute();
    }
}
