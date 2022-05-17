using System.Collections.Generic;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<DataMinerAttribute> GetAllAttributes();

        (DataMinerAttribute Attribute, string DefaultEquation) GetNetworkDefinitionAttribute();
    }
}
