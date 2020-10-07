using System;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class SegmentItemMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Segment entity to Segment domain");
            }

            if (entity.Location == null)
            {
                throw new NullReferenceException("Cannot map null Location entity to Location domain");
            }

            return new MaintainableAsset(entity.Location.ToDomain());
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain, Guid networkId)
        {
            var locationEntity = domain.Location.ToEntity();

            var segmentEntity = new MaintainableAssetEntity
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
