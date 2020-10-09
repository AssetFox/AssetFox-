using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class AttributeDataItemMapper
    {
        public static IAttributeDatum ToDomain<T>(this AttributeDatumEntity<T> entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum entity to AttributeDatum domain");
            }

            if (entity.Discriminator == "NumericAttributeDatum")
            {
                return new AttributeDatum<double>(
                    entity.Attribute.ToDomain(),
                    Convert.ToDouble(entity.Value),
                    entity.Location.ToDomain(),
                    entity.TimeStamp);
            }

            if (entity.Discriminator == "TextAttributeDatum")
            {
                return new AttributeDatum<string>(
                    entity.Attribute.ToDomain(),
                    entity.Value.ToString() ?? throw new InvalidOperationException("Data value for attribute cannot be null"),
                    entity.Location.ToDomain(),
                    entity.TimeStamp);
            }

            throw new InvalidOperationException("Cannot determine AttributeDatum entity type");
        }

        public static IAttributeDatumEntity ToEntity<T>(this AttributeDatum<T> domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum domain to AttributeDatum entity");
            }

            var valueType = typeof(T);

            if (valueType == typeof(double))
            {
                return new AttributeDatumEntity<double>
                {
                    Id = Guid.NewGuid(),
                    Attribute = domain.Attribute.ToEntity(),
                    Location = domain.Location.ToEntity(),
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    Value = Convert.ToDouble(domain.Value)
                };
            }

            if (valueType == typeof(string))
            {
                return new AttributeDatumEntity<string>
                {
                    Id = Guid.NewGuid(),
                    Attribute = domain.Attribute.ToEntity(),
                    Location = domain.Location.ToEntity(),
                    Discriminator = "TextAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    Value = domain.Value.ToString()
                };
            }

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum entity");
        }
    }
}
