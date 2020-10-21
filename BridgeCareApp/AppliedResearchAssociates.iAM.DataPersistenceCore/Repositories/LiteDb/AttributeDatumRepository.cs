using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using LiteDB;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeDatumRepository : LiteDbRepository, IAttributeDatumRepository
    {
        public AttributeDatumRepository(ILiteDbContext context) : base(context) { }

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId) =>
            Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS")
                .Include(_ => _.AttributeDatumEntities)
                .Find(_ => _.NetworkId == networkId)
                .SelectMany(_ => _.AttributeDatumEntities.Select(_ => _.AttributeEntity.ToDomain()))
                .DistinctBy(_ => _.Id);

        // Removes assigned data on maintainable assets in the given network that have attribute data with attributeIds in the provided list.
        public int UpdateAssignedDataByAttributeId(Guid networkId, IEnumerable<Guid> attributeIds, IEnumerable<MaintainableAsset> maintainableAssets)
        {
            var maintainableAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            var maintainableAssetEntities = maintainableAssets.Select(_ => _.ToEntity());

            var attributeDatumEntitiesToUpdate = maintainableAssetEntities.SelectMany(_ => _.AttributeDatumEntities)
                    .Where(_ => attributeIds.Contains(_.AttributeEntity.Id)).ToList();

            return maintainableAssetCollection
                .UpdateMany(_ =>
                new MaintainableAssetEntity()
                {
                    AttributeDatumEntities = attributeDatumEntitiesToUpdate,
                    Id = _.Id,
                    LocationEntity = _.LocationEntity,
                    NetworkId = _.NetworkId
                },
                _ => _.NetworkId == networkId);
        }
    }
}
