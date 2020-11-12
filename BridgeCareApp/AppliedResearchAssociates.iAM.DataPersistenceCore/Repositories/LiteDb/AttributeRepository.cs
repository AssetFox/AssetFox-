using System.Collections.Generic;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeRepository : LiteDbRepository, IAttributeRepository
    {
        public AttributeRepository(ILiteDbContext context) : base(context)
        {
        }

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            // This is a do nothing case for LiteDb.
            return;
        }

        public void AddAttribute(Attribute attribute)
        {
            // This is a do nothing case for LiteDb.
            return;
        }
    }
}
