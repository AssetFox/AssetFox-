using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class SegmentItemMapper
    {
        public static Segment ToDomain(this SegmentEntity entity) =>
            entity == null
                ? new Segment(new SectionLocation("NA"))
                : new Segment(entity.Location == null
                    ? new SectionLocation("NA")
                    : entity.Location.ToDomain());

        public static SegmentEntity ToEntity(this Segment domain, Guid networkId, string uniqueIdentifier)
        {
            var locationId = Guid.NewGuid();

            var segmentEntity = new SegmentEntity
            {
                Id = Guid.NewGuid(),
                NetworkId = networkId,
                UniqueIdentifier = uniqueIdentifier,
                LocationId = locationId,
                Location = domain.Location.ToEntity(locationId)
            };

            var numericValues = domain.AssignedData.Where(d => d.Attribute.DataType == "NUMERIC")
                .Select(d =>
                    domain.GetAggregatedValuesByYear(d.Attribute, AggregationRuleFactory.CreateNumericRule(d.Attribute)))
                .Select(d => d.ToEntity(segmentEntity.Id, locationId));

            var textValues = domain.AssignedData.Where(d => d.Attribute.DataType == "TEXT")
                .Select(d =>
                    domain.GetAggregatedValuesByYear(d.Attribute, AggregationRuleFactory.CreateTextRule(d.Attribute)))
                .Select(d => d.ToEntity(segmentEntity.Id, locationId));

            foreach (var entity in numericValues)
            {
                segmentEntity.AttributeData.Add((AttributeDatumEntity)Convert.ChangeType(entity, typeof(AttributeDatumEntity)));
            }

            return segmentEntity;
        }
    }
}
