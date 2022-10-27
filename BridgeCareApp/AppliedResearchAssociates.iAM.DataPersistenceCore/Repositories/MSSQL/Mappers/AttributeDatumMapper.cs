using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AttributeDatumMapper
    {
        public static IAttributeDatum ToDomain(this AttributeDatumEntity entity, string encryptionKey)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum entity to AttributeDatum domain");
            }

            if (entity.Discriminator == "NumericAttributeDatum")
            {
                return new AttributeDatum<double>(
                    entity.Id,
                    entity.Attribute.ToDomain(encryptionKey),
                    entity.NumericValue ?? 0,
                    entity.AttributeDatumLocation.ToDomain(),
                    entity.TimeStamp);
            }

            if (entity.Discriminator == "TextAttributeDatum")
            {
                return new AttributeDatum<string>(
                    entity.Id,
                    entity.Attribute.ToDomain(encryptionKey),
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

            var entity = new AttributeDatumEntity
            {
                MaintainableAssetId = maintainableAssetId,
                AttributeId = domain.Attribute.Id,
                TimeStamp = domain.TimeStamp
            };

            if (domain is AttributeDatum<double> numericAttributeDatum)
            {
                entity.Id = numericAttributeDatum.Id;
                entity.Discriminator = "NumericAttributeDatum";
                entity.NumericValue = Convert.ToDouble(numericAttributeDatum.Value);
                entity.AttributeDatumLocation =
                    (AttributeDatumLocationEntity)domain.Location.ToEntity(numericAttributeDatum.Id,
                        typeof(AttributeDatumEntity));
                return entity;
            }

            if (domain is AttributeDatum<string> textAttributeDatum)
            {
                entity.Id = textAttributeDatum.Id;
                entity.Discriminator = "TextAttributeDatum";
                entity.TextValue = textAttributeDatum.Value;
                entity.AttributeDatumLocation =
                    (AttributeDatumLocationEntity)domain.Location.ToEntity(textAttributeDatum.Id,
                        typeof(AttributeDatumEntity));
                return entity;
            }

            throw new InvalidOperationException("Could not determine Value data type for AttributeDatum entity");
        }
    }
}
