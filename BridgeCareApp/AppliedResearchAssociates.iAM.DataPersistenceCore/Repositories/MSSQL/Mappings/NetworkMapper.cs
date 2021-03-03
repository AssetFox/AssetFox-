using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

using SimulationAnalysisDomains = AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class NetworkMapper
    {
        public static DataAssignment.Networking.Network ToDomain(this NetworkEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Network entity to Network domain");
            }

            return new DataAssignment.Networking.Network(
                entity.MaintainableAssets == null
                    ? new List<MaintainableAsset>()
                    : entity.MaintainableAssets.Select(e => e.ToDomain()).ToList(),
                entity.Id,
                entity.Name);
        }

        public static NetworkEntity ToEntity(this DataAssignment.Networking.Network domain)
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

        public static NetworkEntity ToEntity(this SimulationAnalysisDomains.Network domain) =>
            new NetworkEntity
            {
                Id = domain.Id,
                Name = domain.Name
            };

        public static NetworkDTO ToDto(this NetworkEntity entity) =>
            new NetworkDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate
            };

        public static SimulationAnalysisDomains.Network ToSimulationAnalysisDomain(this NetworkEntity entity, Explorer explorer)
        {
            var network = explorer.AddNetwork();
            network.Id = entity.Id;
            network.Name = entity.Name;

            if (entity.Facilities.Any())
            {
                entity.Facilities.ForEach(_ => _.CreateFacility(network));
            }

            return network;
        }
    }
}
