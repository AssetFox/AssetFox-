using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeRepository
    {
        void UpsertAttributes(List<DataMiner.Attributes.Attribute> attributes);
        void JoinAttributesWithEquationsAndCriteria(Explorer explorer);
        Explorer GetExplorer();
    }
}
