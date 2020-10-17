using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class NetworkMapper
    {
        public static Network ToDomain(this NetworkEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Network entity to Network domain");
            }

            return new Network(
                entity.MaintainableAssetEntities == null
                    ? new List<MaintainableAsset>()
                    : entity.MaintainableAssetEntities.Select(e => e.ToDomain()).ToList(),
                entity.Id,
                entity.Name);
        }

        public static NetworkEntity ToEntity(this Network domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Network domain to Network entity");
            }

            return new NetworkEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                MaintainableAssetEntities = domain.MaintainableAssets.Any()
                    ? domain.MaintainableAssets.Select(d => d.ToEntity()).ToList()
                    : new List<MaintainableAssetEntity>()
            };
        }
    }
}
