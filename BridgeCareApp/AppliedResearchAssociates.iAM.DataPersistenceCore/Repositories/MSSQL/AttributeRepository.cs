using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<Attribute, AttributeEntity>, IAttributeDataRepository
    public class AttributeRepository : MSSQLRepository<DataMinerAttribute>, IAttributeDataRepository
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public void AddAttributes(List<Attribute> domains) =>
            context.Attributes.AddRange(domains.Select(d => d.ToEntity()));

        public void AddAttribute(Attribute domain) => context.Attributes.Add(domain.ToEntity());
    }
}
