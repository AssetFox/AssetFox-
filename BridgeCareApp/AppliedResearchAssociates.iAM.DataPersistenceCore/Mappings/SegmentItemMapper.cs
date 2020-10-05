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
        public static Segment ToDomain(this SegmentEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Segment entity to Segment domain");
            }

            if (entity.Location == null)
            {
                throw new NullReferenceException("Cannot map null Location entity to Location domain");
            }

            return new Segment(entity.Location.ToDomain());
        }
            

        public static SegmentEntity ToEntity(this Segment domain, Guid networkId)
        {
            var locationEntity = domain.Location.ToEntity();

            var segmentEntity = new SegmentEntity
            {
                Id = Guid.NewGuid(),
                NetworkId = networkId,
                UniqueIdentifier = locationEntity.UniqueIdentifier,
                LocationId = locationEntity.Id,
                Location = locationEntity
            };

            return segmentEntity;
        }
    }
}
