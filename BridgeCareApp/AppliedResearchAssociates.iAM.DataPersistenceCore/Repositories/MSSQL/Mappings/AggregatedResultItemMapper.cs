using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings
{
    public static class AggregatedResultItemMapper
    {
        public static IEnumerable<AggregatedResultEntity> ToEntity<T>(this IEnumerable<(DataMinerAttribute attribute, (int year, T value))> domains)
        {
            if (domains == null || !domains.Any())
            {
                throw new NullReferenceException("Cannot map null AggregatedResult domains to AggregatedResult entities");
            }

            var valueType = typeof(T);

            if (valueType == typeof(double))
            {
                return domains.Select(d => new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    Discriminator = "NumericAggregatedResult",
                    Year = d.Item2.year,
                    NumericValue = (double)Convert.ChangeType(d.Item2.value, typeof(double)),
                    AttributeId = d.attribute.Id
                });
            }

            if (valueType == typeof(string))
            {
                return domains.Select(d => new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    Discriminator = "TextAggregatedResult",
                    Year = d.Item2.year,
                    TextValue = (string)Convert.ChangeType(d.Item2.value, typeof(string)),
                    AttributeId = d.attribute.Id
                });
            }

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum entity");
        }
    }
}
