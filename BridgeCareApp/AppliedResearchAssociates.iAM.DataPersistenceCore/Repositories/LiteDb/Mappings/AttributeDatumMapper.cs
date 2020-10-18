using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class AttributeDatumMapper
    {
        public static IAttributeDatum ToDomain<T>(this IAttributeDatumEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum entity to AttributeDatum domain");
            }

            var attributeDatumEntity = entity as Repositories.LiteDb.Entities.AttributeDatumEntity<T>;

            if (entity.Discriminator == "NumericAttributeDatum")
            {
                return new AttributeDatum<double>(
                    attributeDatumEntity.AttributeEntity.ToDomain(),
                    Convert.ToDouble(attributeDatumEntity.Value),
                    attributeDatumEntity.LocationEntity.ToDomain(),
                    attributeDatumEntity.TimeStamp);
            }

            if (entity.Discriminator == "TextAttributeDatum")
            {
                return new AttributeDatum<string>(
                    attributeDatumEntity.AttributeEntity.ToDomain(),
                    attributeDatumEntity.Value.ToString() ?? throw new InvalidOperationException("Data value for attribute cannot be null"),
                    attributeDatumEntity.LocationEntity.ToDomain(),
                    attributeDatumEntity.TimeStamp);
            }

            throw new InvalidOperationException("Cannot determine AttributeDatum entity type");
        }

        public static IAttributeDatumEntity ToEntity(this IAttributeDatum domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum domain to AttributeDatum entity");
            }

            if (domain is DataMiner.Attributes.AttributeDatum<double> attributeDatumNumericDomain)
            {
                return new Repositories.LiteDb.Entities.AttributeDatumEntity<double>
                {
                    Id = Guid.NewGuid(),
                    AttributeEntity = domain.Attribute.ToEntity(),
                    LocationEntity = domain.Location.ToEntity(),
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    Value = Convert.ToDouble(attributeDatumNumericDomain.Value)
                };
            }

            if (domain is DataMiner.Attributes.AttributeDatum<string> attributeDatumTextDomain)
            {
                return new Repositories.LiteDb.Entities.AttributeDatumEntity<string>
                {
                    Id = Guid.NewGuid(),
                    AttributeEntity = domain.Attribute.ToEntity(),
                    LocationEntity = domain.Location.ToEntity(),
                    Discriminator = "TextAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    Value = attributeDatumTextDomain.Value.ToString()
                };
            }

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum");
        }
    }
}
