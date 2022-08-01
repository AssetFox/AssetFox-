using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<Attribute> GetAllAttributes(System.Guid dataSourceId);

        (Attribute Attribute, string DefaultEquation) GetNetworkDefinitionAttribute(System.Guid dataSourceId);
    }
}
