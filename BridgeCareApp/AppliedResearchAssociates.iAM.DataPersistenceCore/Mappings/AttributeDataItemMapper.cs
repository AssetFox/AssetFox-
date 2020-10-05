using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class AttributeDataItemMapper
    {
        public static List<AttributeDatumEntity> ToEntity(
            this IEnumerable<(Attribute attribute, (int year, double value))> aggregatedResult,
            Guid segmentId, Guid locationId)
        {
            if (aggregatedResult == null || !aggregatedResult.Any())
            {
                throw new NullReferenceException("Cannot map null or empty list of aggregated results to AttributeDatum entity list");
            }

            return aggregatedResult.Select(r =>
                new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    SegmentId = segmentId,
                    LocationId = locationId,
                    AttributeId = r.attribute.Id,
                    NumericValue = r.Item2.value,
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = DateTime.Now
                }).ToList();
        }

        public static List<AttributeDatumEntity> ToEntity(
            this IEnumerable<(Attribute attribute, (int year, string value))> aggregatedResult,
            Guid segmentId, Guid locationId)
        {
            if (aggregatedResult == null || !aggregatedResult.Any())
            {
                throw new NullReferenceException("Cannot map null or empty list of aggregated results to AttributeDatum entity list");
            }

            return aggregatedResult.Select(r =>
                new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    SegmentId = segmentId,
                    LocationId = locationId,
                    AttributeId = r.attribute.Id,
                    TextValue = r.Item2.value,
                    Discriminator = "TextAttributeDatum",
                    TimeStamp = DateTime.Now
                }).ToList();
        }
    }
}
