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
        public static List<NumericAttributeDatumEntity> ToEntity(
            this IEnumerable<(Attribute attribute, (int year, double value))> aggregatedResult,
            Guid segmentId, Guid locationId)
        {
            if (aggregatedResult == null)
            {
                throw new NullReferenceException("Cannot create NumericAttributeDatumEntity without aggregated result values");
            }

            return aggregatedResult.Select(r =>
                new NumericAttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    SegmentId = segmentId,
                    LocationId = locationId,
                    AttributeId = r.attribute.Id,
                    Value = r.Item2.value,
                    TimeStamp = DateTime.Now
                }).ToList();
        }

        public static List<TextAttributeDatumEntity> ToEntity(
            this IEnumerable<(Attribute attribute, (int year, string value))> aggregatedResult,
            Guid segmentId, Guid locationId)
        {
            if (aggregatedResult == null)
            {
                throw new NullReferenceException("Cannot create TextAttributeDatumEntity without aggregated result values");
            }

            return aggregatedResult.Select(r =>
                new TextAttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    SegmentId = segmentId,
                    LocationId = locationId,
                    AttributeId = r.attribute.Id,
                    Value = r.Item2.value,
                    TimeStamp = DateTime.Now
                }).ToList();
        }
    }
}
