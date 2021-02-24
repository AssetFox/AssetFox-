using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
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

        public static TargetConditionGoalEntity ToEntity(this TargetConditionGoalDTO dto, Guid libraryId,
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

        public static TargetConditionGoalLibraryEntity ToEntity(this TargetConditionGoalLibraryDTO dto) =>
            new TargetConditionGoalLibraryEntity {Id = dto.Id, Name = dto.Name, Description = dto.Description};

        public static void CreateTargetConditionGoal(this TargetConditionGoalEntity entity, Simulation simulation)
        {
            var targetConditionGoal = simulation.AnalysisMethod.AddTargetConditionGoal();
            targetConditionGoal.Id = entity.Id;
            targetConditionGoal.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            targetConditionGoal.Target = entity.Target;
            targetConditionGoal.Year = entity.Year;
            targetConditionGoal.Name = entity.Name;
            targetConditionGoal.Criterion.Expression =
                entity.CriterionLibraryTargetConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
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

        public static TargetConditionGoalLibraryDTO ToDto(this TargetConditionGoalLibraryEntity entity) =>
            new TargetConditionGoalLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                TargetConditionGoals = entity.TargetConditionGoals.Any()
                    ? entity.TargetConditionGoals.Select(_ => _.ToDto()).ToList()
                    : new List<TargetConditionGoalDTO>(),
                AppliedScenarioIds = entity.TargetConditionGoalLibrarySimulationJoins.Any()
                    ? entity.TargetConditionGoalLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };
    }
}
