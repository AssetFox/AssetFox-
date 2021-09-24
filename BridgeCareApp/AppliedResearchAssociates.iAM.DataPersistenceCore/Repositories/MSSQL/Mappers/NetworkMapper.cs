using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;
using SimulationAnalysisDomains = AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class NetworkMapper
    {
        public static Network ToDomain(this NetworkEntity entity) =>
            new Network(
                entity.MaintainableAssets.Any()
                    ? entity.MaintainableAssets.Select(e => e.ToDomain()).ToList()
                    : new List<MaintainableAsset>(),
                entity.Id,
                entity.Name);

        public static SimulationAnalysisDomains.Network ToDomain(this NetworkEntity entity, SimulationAnalysisDomains.Explorer explorer)
        {
            var network = explorer.AddNetwork();
            network.Id = entity.Id;
            network.Name = entity.Name;

            if (entity.MaintainableAssets.Any())
            {
                entity.MaintainableAssets.ForEach(_ => _.CreateFacility(network));
            }

            return network;
        }

        public static NetworkEntity ToEntity(this Network domain) =>
            new NetworkEntity {Id = domain.Id, Name = domain.Name};

        public static NetworkEntity ToEntity(this SimulationAnalysisDomains.Network domain) =>
            new NetworkEntity {Id = domain.Id, Name = domain.Name};

        public static NetworkDTO ToDto(this NetworkEntity entity) =>
            new NetworkDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Status = entity.NetworkRollupDetail != null ? entity.NetworkRollupDetail.Status : "N/A",
                BenefitQuantifier = entity.BenefitQuantifier != null
                    ? entity.BenefitQuantifier.ToDto()
                    : new BenefitQuantifierDTO {NetworkId = entity.Id, Equation = new EquationDTO {Id = Guid.NewGuid()}}
            };
    }
}
