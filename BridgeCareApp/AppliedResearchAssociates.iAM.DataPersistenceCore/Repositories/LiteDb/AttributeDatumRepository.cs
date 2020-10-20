using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using LiteDB;
using MoreLinq;

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

        public int UpdateAssignedData(Network network)
        {
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity());

            _ = DeleteAssignedDataFromNetwork(network.Id);

            return Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS")
                .Update(maintainableAssetEntities);
        }

        public int DeleteAssignedDataFromNetwork(Guid networkId)
        {
            var maintenanceAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTENANCE_ASSETS");
            return maintenanceAssetCollection.UpdateMany(_ => new MaintainableAssetEntity()
                {
                    AttributeDatumEntities = new List<IAttributeDatumEntity>(),
                    Id = _.Id,
                    LocationEntity = _.LocationEntity,
                    NetworkId = _.NetworkId
                },
                _ => _.NetworkId == networkId);
        }
    }
}
