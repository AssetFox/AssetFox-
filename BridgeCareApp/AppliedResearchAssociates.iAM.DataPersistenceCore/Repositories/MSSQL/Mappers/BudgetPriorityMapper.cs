using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetPriorityMapper
    {
        public static BudgetPriorityEntity ToEntity(this BudgetPriority domain, Guid budgetPriorityLibraryId) =>
            new BudgetPriorityEntity
            {
                Id = domain.Id,
                BudgetPriorityLibraryId = budgetPriorityLibraryId,
                PriorityLevel = domain.PriorityLevel,
                Year = domain.Year
            };

        public static BudgetPriorityEntity ToEntity(this BudgetPriorityDTO dto, Guid libraryId) =>
            new BudgetPriorityEntity
            {
                Id = dto.Id,
                BudgetPriorityLibraryId = libraryId,
                PriorityLevel = dto.PriorityLevel,
                Year = dto.Year
            };

        public static BudgetPriorityLibraryEntity ToEntity(this BudgetPriorityLibraryDTO dto) =>
            new BudgetPriorityLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static BudgetPriorityDTO ToDto(this BudgetPriorityEntity entity) =>
            new BudgetPriorityDTO
            {
                Id = entity.Id,
                PriorityLevel = entity.PriorityLevel,
                Year = entity.Year,
                BudgetPercentagePairs = entity.BudgetPercentagePairs.Any()
                    ? entity.BudgetPercentagePairs.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetPercentagePairDTO>(),
                CriterionLibrary = entity.CriterionLibraryBudgetPriorityJoin != null
                    ? entity.CriterionLibraryBudgetPriorityJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static BudgetPriorityLibraryDTO ToDto(this BudgetPriorityLibraryEntity entity) =>
            new BudgetPriorityLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BudgetPriorities = entity.BudgetPriorities.Any()
                    ? entity.BudgetPriorities.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetPriorityDTO>(),
                AppliedScenarioIds = entity.BudgetPriorityLibrarySimulationJoins.Any()
                    ? entity.BudgetPriorityLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };

        public static void CreateBudgetPriority(this BudgetPriorityEntity entity, Simulation simulation)
        {
            var priority = simulation.AnalysisMethod.AddBudgetPriority();
            priority.Id = entity.Id;
            priority.PriorityLevel = entity.PriorityLevel;
            priority.Year = entity.Year;
            priority.Criterion.Expression =
                entity.CriterionLibraryBudgetPriorityJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

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
