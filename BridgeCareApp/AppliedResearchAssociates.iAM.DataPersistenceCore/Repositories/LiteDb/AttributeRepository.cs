using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using Microsoft.Extensions.Options;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeRepository : LiteDbRepository, IAttributeRepository
    {
        public AttributeRepository(ILiteDbContext context) : base(context)
        {
        }

        public void CreateMissingAttributes(List<Attribute> attributes)
        {
            // This is a do nothing case for LiteDb.
            return;
        }

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId)
        {
            return Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS")
                .Include(_ => _.AttributeDatumEntities)
                .Find(_ => _.NetworkId == networkId)
                .SelectMany(_ => _.AttributeDatumEntities.Select(_ => _.AttributeEntity.ToDomain()))
                .DistinctBy(_ => _.Id);
        }
    }
}
