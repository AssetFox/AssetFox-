using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TargetConditionGoalMapper
    {
        public static TargetConditionGoalEntity ToEntity(this TargetConditionGoal domain, Guid targetConditionGoalLibraryId, Guid attributeId) =>
            new TargetConditionGoalEntity
            {
                Id = domain.Id,
                TargetConditionGoalLibraryId = targetConditionGoalLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Target = domain.Target,
                Year = domain.Year
            };

        public static ScenarioTargetConditionGoalEntity ToScenarioEntity(this TargetConditionGoal domain, Guid simulatoinId, Guid attributeId) =>
            new ScenarioTargetConditionGoalEntity
            {
                Id = domain.Id,
                SimulationId = simulatoinId,
                AttributeId = attributeId,
                Name = domain.Name,
                Target = domain.Target,
                Year = domain.Year
            };

        public static TargetConditionGoalEntity ToLibraryEntity(this TargetConditionGoalDTO dto, Guid libraryId,
            Guid attributeId) =>
            new TargetConditionGoalEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                TargetConditionGoalLibraryId = libraryId,
                AttributeId = attributeId,
                Target = dto.Target,
                Year = dto.Year
            };

        public static ScenarioTargetConditionGoalEntity ToScenarioEntity(this TargetConditionGoalDTO dto, Guid simulationId,
            Guid attributeId) =>
            new ScenarioTargetConditionGoalEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                LibraryId = dto.LibraryId,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Target = dto.Target,
                Year = dto.Year
            };

        public static TargetConditionGoalLibraryEntity ToEntity(this TargetConditionGoalLibraryDTO dto) =>
            new TargetConditionGoalLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static void CreateTargetConditionGoal(this ScenarioTargetConditionGoalEntity entity, Simulation simulation)
        {
            var targetConditionGoal = simulation.AnalysisMethod.AddTargetConditionGoal();
            targetConditionGoal.Id = entity.Id;
            targetConditionGoal.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            targetConditionGoal.Target = entity.Target;
            targetConditionGoal.Year = entity.Year;
            targetConditionGoal.Name = entity.Name;
            targetConditionGoal.Criterion.Expression =
                entity.CriterionLibraryScenarioTargetConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }

        public static TargetConditionGoalDTO ToDto(this TargetConditionGoalEntity entity) =>
            new TargetConditionGoalDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                Target = entity.Target,
                Year = entity.Year,
                CriterionLibrary = entity.CriterionLibraryTargetConditionGoalJoin != null
                    ? entity.CriterionLibraryTargetConditionGoalJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
        public static TargetConditionGoalDTO ToDto(this ScenarioTargetConditionGoalEntity entity) =>
            new TargetConditionGoalDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                Target = entity.Target,
                Year = entity.Year,
                LibraryId = entity.LibraryId,
                CriterionLibrary = entity.CriterionLibraryScenarioTargetConditionGoalJoin != null
                    ? entity.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static TargetConditionGoalLibraryDTO ToDto(this TargetConditionGoalLibraryEntity entity) =>
            new TargetConditionGoalLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                TargetConditionGoals = entity.TargetConditionGoals.Any()
                    ? entity.TargetConditionGoals.Select(_ => _.ToDto()).ToList()
                    : new List<TargetConditionGoalDTO>()
            };
    }
}
