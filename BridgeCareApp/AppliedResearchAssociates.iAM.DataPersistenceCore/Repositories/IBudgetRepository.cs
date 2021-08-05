using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetRepository
    {
        void CreateBudgetLibrary(string name, Guid simulationId);

        void CreateBudgets(List<Budget> budgets, Guid simulationId);

        List<SimpleBudgetDetailDTO> ScenarioSimpleBudgetDetails(Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibrariesWithBudgets();

        void UpsertBudgetLibrary(BudgetLibraryDTO dto);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId);

        void DeleteBudgetLibrary(Guid libraryId);

        List<BudgetEntity> GetBudgetsWithBudgetAmounts(Guid libraryId);

        BudgetLibraryDTO GetBudgetLibraryWithBudgetsAndBudgetAmounts(Guid libraryId);

        List<BudgetDTO> GetScenarioBudgets(Guid simulationId);

        void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId);
    }
}
