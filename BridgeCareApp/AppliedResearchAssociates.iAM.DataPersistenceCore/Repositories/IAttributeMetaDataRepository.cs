using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<DataMiner.Attributes.Attribute> GetAllAttributes();

        DataMiner.Attributes.Attribute GetNetworkDefinitionAttribute();
    }
}
