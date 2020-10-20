using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings
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

        public static AttributeDatumEntity ToEntity(this IAttributeDatum domain, Guid maintainableAssetId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum domain to AttributeDatum entity");
            }

            if (domain is AttributeDatum<double> numericAttributeDatum)
            {
                return new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    MaintainableAssetId = maintainableAssetId,
                    AttributeId = numericAttributeDatum.Attribute.Id,
                    LocationId = numericAttributeDatum.Location.Id,
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = numericAttributeDatum.TimeStamp,
                    NumericValue = Convert.ToDouble(numericAttributeDatum.Value)
                };
            }

            if (domain is AttributeDatum<string> textAttributeDatum)
            {
                return new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    MaintainableAssetId = maintainableAssetId,
                    AttributeId = textAttributeDatum.Attribute.Id,
                    LocationId = textAttributeDatum.Location.Id,
                    Discriminator = "TextAttributeDatum",
                    TimeStamp = textAttributeDatum.TimeStamp,
                    TextValue = textAttributeDatum.Value
                };
            }

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum entity");
        }
    }
}
