using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class AttributeDataItemMapper
    {

        public static AttributeDatumEntity ToEntity<T>(this AttributeDatum<T> domain, Guid segmentId, Guid locationId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null AttributeDatum domain to AttributeDatum entity");
            }

            var valueType = typeof(T);

            if (valueType == typeof(double))
            {
                return new AttributeDatumEntity
                {
                    Id = Guid.NewGuid(),
                    AttributeId = domain.Attribute.Id,
                    SegmentId = segmentId,
                    LocationId = locationId,
                    Discriminator = "NumericAttributeDatum",
                    TimeStamp = domain.TimeStamp,
                    NumericValue = (double)Convert.ChangeType(domain.Value, typeof(double))
                };
            }

            return new AttributeDatumEntity
            {
                Id = Guid.NewGuid(),
                AttributeId = domain.Attribute.Id,
                SegmentId = segmentId,
                LocationId = locationId,
                Discriminator = "TextAttributeDatum",
                TimeStamp = domain.TimeStamp,
                TextValue = (string)Convert.ChangeType(domain.Value, typeof(string))
            };
        }
    }
}
