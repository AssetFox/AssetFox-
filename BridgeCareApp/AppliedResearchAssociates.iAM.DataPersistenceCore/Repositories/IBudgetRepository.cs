using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetRepository
    {
        void CreateScenarioBudgets(List<Budget> budgets, Guid simulationId);

        List<SimpleBudgetDetailDTO> GetScenarioSimpleBudgetDetails(Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibraries();

        void UpsertBudgetLibrary(BudgetLibraryDTO dto);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId);

        void DeleteBudgetLibrary(Guid libraryId);

        List<BudgetEntity> GetLibraryBudgets(Guid libraryId);

        BudgetLibraryDTO GetBudgetLibrary(Guid libraryId);

        List<BudgetDTO> GetScenarioBudgets(Guid simulationId);

        void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId);
        ScenarioBudgetEntity EnsureExistenceOfUnknownBudgetForSimulation(Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildren();

        List<int> GetBudgetYearsBySimulationId(Guid simulationId);
        Dictionary<string, string> GetCriteriaPerBudgetNameForSimulation(Guid simulationId);
        Dictionary<string, string> GetCriteriaPerBudgetNameForBudgetLibrary(Guid budgetLibraryId);
    }
}
