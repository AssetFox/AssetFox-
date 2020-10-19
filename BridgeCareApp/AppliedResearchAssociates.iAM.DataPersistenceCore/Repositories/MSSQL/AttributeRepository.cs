using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<Attribute>
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public override void AddAll(IEnumerable<Attribute> domains, params object[] args) =>
            context.Attributes.AddRange(domains.Select(d => d.ToEntity()));

        public override void Add(Attribute domain) => context.Attributes.Add(domain.ToEntity());
    }
}
