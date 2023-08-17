using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;

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
        public static ScenarioRemainingLifeLimitEntity ToScenarioEntity(this RemainingLifeLimit domain,
            Guid simulationId, Guid attributeId) =>
            new ScenarioRemainingLifeLimitEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Value = domain.Value
            };

        public static RemainingLifeLimitEntity ToLibraryEntity(this RemainingLifeLimitDTO dto, Guid libraryId,
            Guid attributeId) =>
            new RemainingLifeLimitEntity
            {
                Id = dto.Id,
                RemainingLifeLimitLibraryId = libraryId,
                AttributeId = attributeId,
                Value = dto.Value
            };
        public static ScenarioRemainingLifeLimitEntity ToScenarioEntity(this RemainingLifeLimitDTO dto, Guid simulationId,
            Guid attributeId) =>
            new ScenarioRemainingLifeLimitEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                AttributeId = attributeId,
                LibraryId = dto.LibraryId,
                IsModified = dto.IsModified,
                Value = dto.Value,
            };

        public static ScenarioRemainingLifeLimitEntity ToScenarioEntityWithCriterionLibraryJoin(this RemainingLifeLimitDTO dto, Guid simulationId,
            Guid attributeId)
        {
            //MD Detect if we want the CritionLibrary or not
            var entity = ToScenarioEntity(dto, simulationId,attributeId);
            var criterionLibraryDto = dto.CriterionLibrary;
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = new CriterionLibraryEntity
                {
                    MergedCriteriaExpression = criterionLibraryDto.MergedCriteriaExpression,
                    Id = criterionLibraryDto.Id,
                    Name = criterionLibraryDto.Name,
                };
                
                var join = new CriterionLibraryScenarioRemainingLifeLimitEntity
                {
                    ScenarioRemainingLifeLimitId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                entity.CriterionLibraryScenarioRemainingLifeLimitJoin = join;
            }
            return entity;
        }


        public static RemainingLifeLimitLibraryEntity ToEntity(this RemainingLifeLimitLibraryDTO dto) =>
            new RemainingLifeLimitLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static void CreateRemainingLifeLimit(this ScenarioRemainingLifeLimitEntity entity, Simulation simulation)
        {
            var limit = simulation.AnalysisMethod.AddRemainingLifeLimit();
            limit.Id = entity.Id;
            limit.Value = entity.Value;
            limit.Criterion.Expression =
                entity.CriterionLibraryScenarioRemainingLifeLimitJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

            if (entity.Attribute != null)
            {
                limit.Attribute = simulation.Network.Explorer.NumericAttributes
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
        public static RemainingLifeLimitDTO ToDto(this ScenarioRemainingLifeLimitEntity entity) =>
            new RemainingLifeLimitDTO
            {
                Id = entity.Id,
                Value = entity.Value,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                LibraryId = entity.LibraryId,
                IsModified = entity.IsModified,
                CriterionLibrary = entity.CriterionLibraryScenarioRemainingLifeLimitJoin != null
                    ? entity.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static RemainingLifeLimitLibraryDTO ToDto(this RemainingLifeLimitLibraryEntity entity) =>
            new RemainingLifeLimitLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                RemainingLifeLimits = entity.RemainingLifeLimits.Any()
                    ? entity.RemainingLifeLimits.Select(_ => _.ToDto()).ToList()
                    : new List<RemainingLifeLimitDTO>()
            };
    }
}
