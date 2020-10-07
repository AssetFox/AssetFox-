using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class NetworkItemMapper
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

            return new NetworkEntity { Id = domain.Id, Name = domain.Name };
        }

        public static void UpdateEntity(this NetworkEntity entity, Network domain) => entity.Name = domain.Name;
    }
}
