using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
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
                SectionMapper mapper = new(network);
                entity.MaintainableAssets.ForEach(mapper.CreateMaintainableAsset);
            }

            return network;
        }

        public static NetworkEntity ToEntity(this Network domain) =>
            new NetworkEntity { Id = domain.Id, Name = domain.Name, KeyAttributeId = domain.KeyAttributeId };

        public static NetworkEntity ToEntity(this SimulationAnalysisDomains.Network domain) =>
            new NetworkEntity { Id = domain.Id, Name = domain.Name };

        public static NetworkDTO ToDto(this NetworkEntity entity, List<AttributeEntity> attributeList)
        {
            var dto = new NetworkDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Status = entity.NetworkRollupDetail != null ? entity.NetworkRollupDetail.Status : "N/A",
                BenefitQuantifier = entity.BenefitQuantifier != null
                    ? entity.BenefitQuantifier.ToDto()
                    : new BenefitQuantifierDTO { NetworkId = entity.Id, Equation = new EquationDTO { Id = Guid.NewGuid() } },
                KeyAttribute = entity.KeyAttributeId,
                Attributes = new List<AttributeDTO>()
            };
            foreach (var join in entity.AttributeJoins)
            {
                var networkAttribute = attributeList.FirstOrDefault(_ => _.Id == join.AttributeId);
                if (networkAttribute != null)
                {
                    dto.Attributes.Add(networkAttribute.ToDto());
                }
            }
            return dto;
        }

        public static NetworkEntity ToEntity(this NetworkDTO dto) =>
            new NetworkEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                CreatedDate = dto.CreatedDate,
                LastModifiedDate = dto.LastModifiedDate,
                KeyAttributeId = dto.KeyAttribute
            };
    }
}
