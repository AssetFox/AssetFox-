﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetPriorityMapper
    {
        public static ScenarioBudgetPriorityEntity ToScenarioEntity(this BudgetPriority domain, Guid simulationId) =>
            new ScenarioBudgetPriorityEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                PriorityLevel = domain.PriorityLevel,
                Year = domain.Year
            };

        public static ScenarioBudgetPriorityEntity ToScenarioEntity(this BudgetPriorityDTO dto, Guid simulationId) =>
            new ScenarioBudgetPriorityEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                LibraryId = dto.libraryId,
                IsModified = dto.IsModified,
                PriorityLevel = dto.PriorityLevel,
                Year = dto.Year
            };

        public static ScenarioBudgetPriorityEntity ToScenarioEntityWithCriterionLibraryJoin(this BudgetPriorityDTO dto, Guid simulationId, BaseEntityProperties baseEntityProperties)
        {

            var entity = ToScenarioEntity(dto, simulationId);
            var criterionLibraryDto = dto.CriterionLibrary;
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = CriterionMapper.ToSingleUseEntity(criterionLibraryDto, baseEntityProperties);
                var join = new CriterionLibraryScenarioBudgetPriorityEntity
                {
                    ScenarioBudgetPriorityId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioBudgetPriorityJoin = join;
            }
            var budgetPercentagePairEntities = new List<BudgetPercentagePairEntity>();
            foreach (var budgetPercentagePair in dto.BudgetPercentagePairs)
            {
                var budgetPercentagePairEntity = budgetPercentagePair.ToEntity(entity.Id, baseEntityProperties);
                budgetPercentagePairEntities.Add(budgetPercentagePairEntity);
            }
            entity.BudgetPercentagePairs = budgetPercentagePairEntities;
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;

        }


        public static BudgetPriorityEntity ToLibraryEntity(this BudgetPriorityDTO dto, Guid libraryId) =>
            new BudgetPriorityEntity
            {
                Id = dto.Id,
                BudgetPriorityLibraryId = libraryId,
                PriorityLevel = dto.PriorityLevel,
                Year = dto.Year
            };

        public static BudgetPriorityLibraryEntity ToEntity(this BudgetPriorityLibraryDTO dto) =>
            new BudgetPriorityLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static BudgetPriorityDTO ToDto(this BudgetPriorityEntity entity) =>
            new BudgetPriorityDTO
            {
                Id = entity.Id,
                PriorityLevel = entity.PriorityLevel,
                Year = entity.Year,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
                CriterionLibrary = entity.CriterionLibraryBudgetPriorityJoin != null
                    ? entity.CriterionLibraryBudgetPriorityJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static BudgetPriorityDTO ToDto(this ScenarioBudgetPriorityEntity entity) =>
            new BudgetPriorityDTO
            {
                Id = entity.Id,
                PriorityLevel = entity.PriorityLevel,
                Year = entity.Year,
                libraryId= entity.LibraryId,
                IsModified = entity.IsModified,
                BudgetPercentagePairs = entity.BudgetPercentagePairs.Any()
                    ? entity.BudgetPercentagePairs.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetPercentagePairDTO>(),
                CriterionLibrary = entity.CriterionLibraryScenarioBudgetPriorityJoin != null
                    ? entity.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static BudgetPriorityLibraryDTO ToDto(this BudgetPriorityLibraryEntity entity) =>
            new BudgetPriorityLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                BudgetPriorities = entity.BudgetPriorities.Any()
                    ? entity.BudgetPriorities.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetPriorityDTO>(),
            };

        public static void CreateBudgetPriority(this ScenarioBudgetPriorityEntity entity, Simulation simulation)
        {
            var priority = simulation.AnalysisMethod.AddBudgetPriority();
            priority.Id = entity.Id;
            priority.PriorityLevel = entity.PriorityLevel;
            priority.Year = entity.Year;
            priority.Criterion.Expression =
                entity.CriterionLibraryScenarioBudgetPriorityJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            if (entity.BudgetPercentagePairs.Any())
            {
                entity.BudgetPercentagePairs.ForEach(_ =>
                {
                    _.FillBudgetPercentagePair(simulation.InvestmentPlan, priority);
                });
            }
        }
    }
}
