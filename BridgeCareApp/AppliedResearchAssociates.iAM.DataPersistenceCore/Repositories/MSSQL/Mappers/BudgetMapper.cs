using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetMapper
    {
        public static BudgetEntity ToEntity(this Budget domain, Guid budgetLibraryId) =>
            new BudgetEntity { Id = domain.Id, BudgetLibraryId = budgetLibraryId, Name = domain.Name };

        public static BudgetEntity ToEntity(this BudgetDTO dto, Guid libraryId) =>
            new BudgetEntity { Id = dto.Id, BudgetLibraryId = libraryId, Name = dto.Name };

        public static BudgetLibraryEntity ToEntity(this BudgetLibraryDTO dto) =>
            new BudgetLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static BudgetDTO ToDto(this BudgetEntity entity) =>
            new BudgetDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                BudgetAmounts = entity.BudgetAmounts.Any()
                    ? entity.BudgetAmounts.Select(_ => _.ToDto(entity.Name)).ToList()
                    : new List<BudgetAmountDTO>(),
                CriterionLibrary = entity.CriterionLibraryBudgetJoin != null
                    ? entity.CriterionLibraryBudgetJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static BudgetLibraryDTO ToDto(this BudgetLibraryEntity entity) =>
            new BudgetLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Budgets = entity.Budgets.Any()
                    ? entity.Budgets.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetDTO>(),
                AppliedScenarioIds = entity.BudgetLibrarySimulationJoins.Any()
                    ? entity.BudgetLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };
    }
}
