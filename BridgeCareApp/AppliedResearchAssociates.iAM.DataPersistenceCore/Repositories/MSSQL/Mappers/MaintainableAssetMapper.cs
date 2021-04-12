using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

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
                //Area = domain.SpatialWeighting.Area,
                //AreaUnit = domain.SpatialWeighting.AreaUnit
            };

        public static MaintainableAssetEntity ToEntity(this Section section, Guid networkId) =>
            new MaintainableAssetEntity
            {
                Id = section.Id,
                NetworkId = networkId,
                SpatialWeighting = section.SpatialWeighting.Expression,
                //Area = section.Area,
                //AreaUnit = section.AreaUnit,
                FacilityName = section.Facility.Name,
                SectionName = section.Name
            };
    }
}
