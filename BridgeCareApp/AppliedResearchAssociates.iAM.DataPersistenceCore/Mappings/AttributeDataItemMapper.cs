using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class AttributeDataItemMapper
    {
public static IAttributeDatum ToDomain(this AttributeDatumEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum entity to AttributeDatum domain");
            }

            if (entity.Discriminator == "NumericAttributeDatum")
            {
                return new AttributeDatum<double>(
                    entity.Attribute.ToDomain(),
                    entity.NumericValue ?? 0,
                    entity.Location.ToDomain(),
                    entity.TimeStamp);
            }

            if (entity.Discriminator == "TextAttributeDatum")
            {
                return new AttributeDatum<string>(
                    entity.Attribute.ToDomain(),
                    entity.TextValue ?? "",
                    entity.Location.ToDomain(),
                    entity.TimeStamp);
            }

            throw new InvalidOperationException("Cannot determine AttributeDatum entity type");
        }

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
                    NumericValue = domain.Value != null ? (double)Convert.ChangeType(domain.Value, typeof(double)) : 0
                };
            }

            if (valueType == typeof(string))
            {
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

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum entity");
        }
    }
}
