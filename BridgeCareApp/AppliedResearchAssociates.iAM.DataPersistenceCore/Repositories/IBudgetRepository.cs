using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetRepository
    {
        void CreateBudgetLibrary(string name, Guid simulationId);

        void CreateBudgets(List<Budget> budgets, Guid simulationId);

        List<SimpleBudgetDetailDTO> ScenarioSimpleBudgetDetails(Guid simulationId);

        List<BudgetLibraryDTO> BudgetLibrariesWithBudgets();

        void UpsertPermitted(Guid simulationId, BudgetLibraryDTO dto);

        void UpsertBudgetLibrary(BudgetLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId);

        void DeleteBudgetLibrary(Guid libraryId);
    }
}
