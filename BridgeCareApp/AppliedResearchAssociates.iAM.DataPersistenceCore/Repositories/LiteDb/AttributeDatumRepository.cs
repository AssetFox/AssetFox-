using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Mappings;
using MoreLinq;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeDatumRepository : LiteDbRepository, IAttributeDatumRepository
    {
        public AttributeDatumRepository(ILiteDbContext context) : base(context) { }

        public IEnumerable<DataMinerAttribute> GetAttributesFromNetwork(Guid networkId) =>
            Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS")
                .Include(_ => _.AttributeDatumEntities)
                .Find(_ => _.NetworkId == networkId)
                .SelectMany(_ => _.AttributeDatumEntities.Select(_ => _.AttributeEntity.ToDomain()))
                .DistinctBy(_ => _.Id);

        // Removes assigned data on maintainable assets in the given network that have attribute
        // data with attributeIds in the provided list.
        public int AddAssignedData(List<MaintainableAsset> assignedMaintainableAssets)
        {
            var maintainableAssetEntityCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            return maintainableAssetEntityCollection.Update(assignedMaintainableAssets.Select(_ => _.ToEntity()));
        }
    }
}
