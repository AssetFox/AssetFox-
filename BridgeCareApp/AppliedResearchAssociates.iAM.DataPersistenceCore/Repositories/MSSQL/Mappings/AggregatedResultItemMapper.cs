using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings
{
    public static class AggregatedResultItemMapper
    {
        public static IEnumerable<AggregatedResultEntity> ToEntity(this IAggregatedResult domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AggregatedResult domains to AggregatedResult entities");
            }

            if(domain is AggregatedResult<double> numericAggregationResult)
            {
                return numericAggregationResult.AggregatedData.Select(d => new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    Discriminator = "TextAggregatedResult",
                    Year = d.yearValuePair.year,
                    NumericValue = Convert.ToDouble(d.yearValuePair.value),
                    AttributeId = d.attribute.Id
                });
            }

            if(domain is AggregatedResult<string> textAggregationResult)
            {
                return textAggregationResult.AggregatedData.Select(d => new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    Discriminator = "TextAggregatedResult",
                    Year = d.yearValuePair.year,
                    TextValue = d.yearValuePair.value.ToString(),
                    AttributeId = d.attribute.Id
                });
            }

            throw new InvalidOperationException("Unable to determine Value data type for AttributeDatum entity");
        }
    }
}
