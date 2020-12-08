using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using DataMiner = AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeRepository : LiteDbRepository, IAttributeRepository
    {
        public AttributeRepository(ILiteDbContext context) : base(context)
        {
        }

        public void UpsertAttributes(List<DataMiner.Attributes.Attribute> attributes)
        {
            // This is a do nothing case for LiteDb.
            return;
        }

        public void JoinAttributesWithEquationsAndCriteria(Explorer explorer)
        {
            // This is a do nothing case for LiteDb.
            return;
        }

        public Explorer GetExplorer()
        {
            // This is a do nothing case for LiteDb.
            return new Explorer();
        }

        public void AddAttribute(DataMiner.Attributes.Attribute attribute)
        {
            // This is a do nothing case for LiteDb.
            return;
        }
    }
}
