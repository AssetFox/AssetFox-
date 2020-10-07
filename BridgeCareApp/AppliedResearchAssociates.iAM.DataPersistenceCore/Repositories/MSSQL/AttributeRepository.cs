using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<DataMinerAttribute, AttributeEntity>, IAttributeDataRepository
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public void AddAttribute(DataMinerAttribute domain) => context.Attributes.Add(domain.ToEntity());
    }
}
