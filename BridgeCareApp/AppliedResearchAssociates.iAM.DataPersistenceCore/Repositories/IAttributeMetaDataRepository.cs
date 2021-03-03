using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<DataMiner.Attributes.Attribute> GetAllAttributes();

        DataMiner.Attributes.Attribute GetNetworkDefinitionAttribute();
    }
}
