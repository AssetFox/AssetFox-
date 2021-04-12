﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class RemainingLifeLimitMapper
    {
        public static RemainingLifeLimitEntity ToEntity(this RemainingLifeLimit domain,
            Guid remainingLifeLimitLibraryId, Guid attributeId) =>
            new RemainingLifeLimitEntity
            {
                Id = domain.Id,
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                AttributeId = attributeId,
                Value = domain.Value
            };

        public static RemainingLifeLimitEntity ToEntity(this RemainingLifeLimitDTO dto, Guid libraryId,
            Guid attributeId) =>
            new RemainingLifeLimitEntity
            {
                Id = dto.Id,
                RemainingLifeLimitLibraryId = libraryId,
                AttributeId = attributeId,
                Value = dto.Value
            };

        public static RemainingLifeLimitLibraryEntity ToEntity(this RemainingLifeLimitLibraryDTO dto) =>
            new RemainingLifeLimitLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static void CreateRemainingLifeLimit(this RemainingLifeLimitEntity entity, Simulation simulation)
        {
            var limit = simulation.AnalysisMethod.AddRemainingLifeLimit();
            limit.Id = entity.Id;
            limit.Value = entity.Value;
            limit.Criterion.Expression =
                entity.CriterionLibraryRemainingLifeLimitJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

            if (entity.Attribute != null)
            {
                limit.Attribute = simulation.Network.Explorer.NumberAttributes
                    .Single(_ => _.Name == entity.Attribute.Name);
            }
        }

        public static RemainingLifeLimitDTO ToDto(this RemainingLifeLimitEntity entity) =>
            new RemainingLifeLimitDTO
            {
                Id = entity.Id,
                Value = entity.Value,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                CriterionLibrary = entity.CriterionLibraryRemainingLifeLimitJoin != null
                    ? entity.CriterionLibraryRemainingLifeLimitJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static RemainingLifeLimitLibraryDTO ToDto(this RemainingLifeLimitLibraryEntity entity) =>
            new RemainingLifeLimitLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                RemainingLifeLimits = entity.RemainingLifeLimits.Any()
                    ? entity.RemainingLifeLimits.Select(_ => _.ToDto()).ToList()
                    : new List<RemainingLifeLimitDTO>(),
                AppliedScenarioIds = entity.RemainingLifeLimitLibrarySimulationJoins.Any()
                    ? entity.RemainingLifeLimitLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };
    }
}