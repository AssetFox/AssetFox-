using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class AttributeDataItemMapper
    {
        public static AttributeDatumEntity ToEntity<T>(this AttributeDatum<T> domain, Guid maintainableAssetId, Guid locationId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum domain to AttributeDatum entity");
            }

            var valueType = typeof(T);

            if (valueType == typeof(double))
            {
                return new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    AttributeId = domain.Attribute.Id,
                    MaintainableAssetId = maintainableAssetId,
                    LocationId = locationId,
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    NumericValue = (double)Convert.ChangeType(domain.Value, typeof(double))
                };
            }

            return new AttributeDatumEntity
            {
                Id = Guid.NewGuid(),
                AttributeId = domain.Attribute.Id,
                MaintainableAssetId = maintainableAssetId,
                LocationId = locationId,
                Discriminator = "TextAttributeDatum",
                TimeStamp = domain.TimeStamp,
                TextValue = (string)Convert.ChangeType(domain.Value, typeof(string))
            };
        }
    }
}
