using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
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
                entity.MaintainableAssets == null
                    ? new List<MaintainableAsset>()
                    : entity.MaintainableAssets.Select(e => e.ToDomain()).ToList(),
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
                MaintainableAssets = domain.MaintainableAssets.Any()
                    ? domain.MaintainableAssets.Select(d => d.ToEntity(domain.Id)).ToList()
                    : new List<MaintainableAssetEntity>()
            };
        }
    }
}
