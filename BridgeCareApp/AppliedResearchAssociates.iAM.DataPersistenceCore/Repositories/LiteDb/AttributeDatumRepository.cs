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
    public class AttributeDatumRepository<T> : LiteDbRepository<IAttributeDatum, IAttributeDatum>, IAttributeDatumRepository
    {
        public AttributeDatumRepository(ILiteDbContext context) : base(context) { }

        private int DeleteAssignedDataFromNetwork(Guid networkId)
        {
            var maintenanceAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTENANCE_ASSETS");
            return maintenanceAssetCollection.UpdateMany(_ => new MaintainableAssetEntity()
            {
                AttributeDatumEntities = new List<IAttributeDatumEntity>() { },
                Id = _.Id,
                LocationEntity = _.LocationEntity,
                NetworkId = _.NetworkId
            },
            _ => _.NetworkId == networkId);
        }

        public int UpdateAssignedData(Network network)
        {
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity());

            _ = DeleteAssignedDataFromNetwork(network.Id);

            var maintainableAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");

            return maintainableAssetCollection.Update(maintainableAssetEntities);
        }

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId)
        {
            var maintainableAssetsCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            var toReturn = maintainableAssetsCollection
                .Include(_ => _.AttributeDatumEntities)
                .Find(_ => _.NetworkId == networkId)
                .SelectMany(_ => _.AttributeDatumEntities.Select(_ => _.AttributeEntity.ToDomain()))
                .DistinctBy(_ => _.Id);
            return toReturn;
        }

        protected override IAttributeDatum ToDomain(IAttributeDatum dataEntity)
        {
            throw new NotImplementedException();
        }

        protected override IAttributeDatum ToEntity(IAttributeDatum domainModel)
        {
            throw new NotImplementedException();
        }
    }
}
