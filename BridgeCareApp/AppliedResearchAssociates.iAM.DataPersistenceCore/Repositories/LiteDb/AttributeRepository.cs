using System.Collections.Generic;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeRepository : LiteDbRepository, IAttributeRepository
    {
        public AttributeRepository(ILiteDbContext context) : base(context)
        {
        }

        public void UpsertAttributes(List<Attribute> attributes)
        {
            // This is a do nothing case for LiteDb.
            return;
        }
    }
}
