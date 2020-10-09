using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<Attribute>
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public override void AddAll(IEnumerable<Attribute> domains, params object[] args) =>
            context.Attributes.AddRange(domains.Select(d => d.ToEntity()));

        public override System.Guid Add(Attribute domain) => context.Attributes.Add(domain.ToEntity()).Entity.Id;
    }
}
