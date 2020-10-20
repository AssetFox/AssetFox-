using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository, IAttributeRepository
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public void CreateAttributes(IEnumerable<Attribute> attributes)
        {
            Context.Attributes.AddRange(attributes.Select(_ => _.ToEntity()));
            Context.SaveChanges();
        }
    }
}
