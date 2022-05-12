using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class MaintainableAssetMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity)
        {
            var maintainableAsset = new MaintainableAsset(
                entity.Id, entity.NetworkId, entity.MaintainableAssetLocation.ToDomain(), entity.SpatialWeighting);

            if (entity.AssignedData != null && entity.AssignedData.Any())
            {
                if (entity.AssignedData.Any(a => a.Discriminator == "NumericAttributeDatum"))
                {
                    var numericAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "NumericAttributeDatum")
                        .Select(e => e.ToDomain());

                    maintainableAsset.AssignAttributeDataFromDataSource(numericAttributeData);
                }

                if (entity.AssignedData.Any(a => a.Discriminator == "TextAttributeDatum"))
                {
                    var textAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "TextAttributeDatum")
                        .Select(e => e.ToDomain());

                    maintainableAsset.AssignAttributeDataFromDataSource(textAttributeData);
                }
            }

            return maintainableAsset;
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain, Guid networkId) =>
            new MaintainableAssetEntity
            {
                Id = domain.Id,
                NetworkId = networkId,
                SpatialWeighting = domain.SpatialWeighting
            };
    }
}
