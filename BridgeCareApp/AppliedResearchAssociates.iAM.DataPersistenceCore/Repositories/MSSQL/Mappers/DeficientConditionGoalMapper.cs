using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
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
        public static ScenarioDeficientConditionGoalEntity ToScenarioEntity(this DeficientConditionGoal domain, Guid simulatoinId, Guid attributeId) =>
            new ScenarioDeficientConditionGoalEntity
            {
                Id = domain.Id,
                SimulationId = simulatoinId,
                AttributeId = attributeId,
                Name = domain.Name,
                AllowedDeficientPercentage = domain.AllowedDeficientPercentage,
                DeficientLimit = domain.DeficientLimit
            };

        public static DeficientConditionGoalEntity ToLibraryEntity(this DeficientConditionGoalDTO dto, Guid libraryId, Guid attributeId) =>
            new DeficientConditionGoalEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                DeficientConditionGoalLibraryId = libraryId,
                AttributeId = attributeId,
                AllowedDeficientPercentage = dto.AllowedDeficientPercentage,
                DeficientLimit = dto.DeficientLimit
            };
        public static ScenarioDeficientConditionGoalEntity ToScenarioEntity(this DeficientConditionGoalDTO dto, Guid simulationId,
            Guid attributeId) =>
            new ScenarioDeficientConditionGoalEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                LibraryId = dto.LibraryId,
                IsModified = dto.IsModified,
                SimulationId = simulationId,
                AttributeId = attributeId,
                AllowedDeficientPercentage = dto.AllowedDeficientPercentage,
                DeficientLimit = dto.DeficientLimit
            };


        public static ScenarioDeficientConditionGoalEntity ToScenarioEntityWithCriterionLibraryJoin(this DeficientConditionGoalDTO dto, Guid simulationId,
          Guid attributeId, BaseEntityProperties baseEntityProperties)
        {

            var entity = ToScenarioEntity(dto, simulationId, attributeId);
            var criterionLibraryDto = dto.CriterionLibrary;
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);

                var join = new CriterionLibraryScenarioDeficientConditionGoalEntity
                {
                    ScenarioDeficientConditionGoalId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioDeficientConditionGoalJoin = join;
            }
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }


        public static DeficientConditionGoalLibraryEntity ToEntity(this DeficientConditionGoalLibraryDTO dto) =>
            new DeficientConditionGoalLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static void CreateDeficientConditionGoal(this ScenarioDeficientConditionGoalEntity entity, Simulation simulation)
        {
            var deficientConditionGoal = simulation.AnalysisMethod.AddDeficientConditionGoal();
            deficientConditionGoal.Id = entity.Id;
            deficientConditionGoal.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            deficientConditionGoal.AllowedDeficientPercentage = entity.AllowedDeficientPercentage;
            deficientConditionGoal.DeficientLimit = entity.DeficientLimit;
            deficientConditionGoal.Name = entity.Name;
            deficientConditionGoal.Criterion.Expression =
                entity.CriterionLibraryScenarioDeficientConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
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
        public static DeficientConditionGoalDTO ToDto(this ScenarioDeficientConditionGoalEntity entity) =>
            new DeficientConditionGoalDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute.Name,
                LibraryId = entity.LibraryId,
                IsModified = entity.IsModified,
                AllowedDeficientPercentage = entity.AllowedDeficientPercentage,
                DeficientLimit = entity.DeficientLimit,
                CriterionLibrary = entity.CriterionLibraryScenarioDeficientConditionGoalJoin != null
                    ? entity.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static DeficientConditionGoalLibraryDTO ToDto(this DeficientConditionGoalLibraryEntity entity) =>
            new DeficientConditionGoalLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                DeficientConditionGoals = entity.DeficientConditionGoals.Any()
                    ? entity.DeficientConditionGoals.Select(_ => _.ToDto()).ToList()
                    : new List<DeficientConditionGoalDTO>()
            };
    }
}
