using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

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

        public Task<List<AttributeDTO>> Attributes() => throw new System.NotImplementedException();

        public void AddAttribute(DataMiner.Attributes.Attribute attribute)
        {
            // This is a do nothing case for LiteDb.
            return;
        }
    }
}
