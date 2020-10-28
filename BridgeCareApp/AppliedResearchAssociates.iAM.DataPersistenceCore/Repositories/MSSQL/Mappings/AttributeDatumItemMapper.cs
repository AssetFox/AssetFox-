using System;
using System.Dynamic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class AttributeDatumItemMapper
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
                    entity.AttributeDatumLocation.ToDomain(),
                    entity.TimeStamp);
            }

            if (entity.Discriminator == "TextAttributeDatum")
            {
                return new AttributeDatum<string>(
                    entity.Attribute.ToDomain(),
                    entity.TextValue ?? "",
                    entity.AttributeDatumLocation.ToDomain(),
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

            if (domain.Location == null)
            {
                throw new NullReferenceException("No location found for assigned data");
            }

            var id = Guid.NewGuid();

            var entity = new AttributeDatumEntity
            {
                Id = id,
                MaintainableAssetId = maintainableAssetId,
                AttributeId = domain.Attribute.Id,
                TimeStamp = domain.TimeStamp,
                AttributeDatumLocation = (AttributeDatumLocationEntity)domain.Location.ToEntity(id, "AttributeDatumEntity")
            };

            if (domain is AttributeDatum<double> numericAttributeDatum)
            {
                entity.Discriminator = "NumericAttributeDatum";
                entity.NumericValue = Convert.ToDouble(numericAttributeDatum.Value);
                return entity;
            }

            if (domain is AttributeDatum<string> textAttributeDatum)
            {
                entity.Discriminator = "TextAttributeDatum";
                entity.TextValue = textAttributeDatum.Value;
                return entity;
            }

            throw new InvalidOperationException("Could not determine Value data type for AttributeDatum entity");
        }
    }
}
