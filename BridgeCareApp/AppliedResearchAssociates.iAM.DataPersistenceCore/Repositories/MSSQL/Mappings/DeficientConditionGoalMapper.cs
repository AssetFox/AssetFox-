using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class DeficientConditionGoalMapper
    {
        public static DeficientConditionGoalEntity ToEntity(this DeficientConditionGoal domain, Guid deficientConditionGoalLibraryId, Guid attributeId) =>
            new DeficientConditionGoalEntity
            {
                Id = domain.Id,
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                AllowedDeficientPercentage = domain.AllowedDeficientPercentage,
                DeficientLimit = domain.DeficientLimit
            };

        public static DeficientConditionGoalEntity ToEntity(this DeficientConditionGoalDTO dto, Guid libraryId, Guid attributeId) =>
            new DeficientConditionGoalEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                DeficientConditionGoalLibraryId = libraryId,
                AttributeId = attributeId,
                AllowedDeficientPercentage = dto.AllowedDeficientPercentage,
                DeficientLimit = dto.DeficientLimit
            };

        public static DeficientConditionGoalLibraryEntity ToEntity(this DeficientConditionGoalLibraryDTO dto) =>
            new DeficientConditionGoalLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static void CreateDeficientConditionGoal(this DeficientConditionGoalEntity entity, Simulation simulation)
        {
            var deficientConditionGoal = simulation.AnalysisMethod.AddDeficientConditionGoal();
            deficientConditionGoal.Id = entity.Id;
            deficientConditionGoal.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            deficientConditionGoal.AllowedDeficientPercentage = entity.AllowedDeficientPercentage;
            deficientConditionGoal.DeficientLimit = entity.DeficientLimit;
            deficientConditionGoal.Name = entity.Name;
            deficientConditionGoal.Criterion.Expression =
                entity.CriterionLibraryDeficientConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }

        public static DeficientConditionGoalDTO ToDto(this DeficientConditionGoalEntity entity) =>
            new DeficientConditionGoalDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute.Name,
                AllowedDeficientPercentage = entity.AllowedDeficientPercentage,
                DeficientLimit = entity.DeficientLimit,
                CriterionLibrary = entity.CriterionLibraryDeficientConditionGoalJoin != null
                    ? entity.CriterionLibraryDeficientConditionGoalJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static DeficientConditionGoalLibraryDTO ToDto(this DeficientConditionGoalLibraryEntity entity) =>
            new DeficientConditionGoalLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                DeficientConditionGoals = entity.DeficientConditionGoals.Any()
                    ? entity.DeficientConditionGoals.Select(_ => _.ToDto()).ToList()
                    : new List<DeficientConditionGoalDTO>(),
                AppliedScenarioIds = entity.DeficientConditionGoalLibrarySimulationJoins.Any()
                    ? entity.DeficientConditionGoalLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };
    }
}
