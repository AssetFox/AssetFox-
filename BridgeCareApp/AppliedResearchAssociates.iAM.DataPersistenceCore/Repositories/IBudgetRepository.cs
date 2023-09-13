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

        /// <summary>
        /// If the library does not exist, adds it. If it does exist, updates the existing one.
        /// </summary>
        /// <param name="dto">the desired new state of the library</param>
        /// <param name="userListModificationIsAllowed">Indicates whether or not the call is allowed to update the user access list of the library. If this is false, and the proposed update does change the access list, an exception will be thrown. Ignored if the call is an insert.</param>
        /// <summary>If this call is an insert, userListModificationIsAllowed is ignored.</summary>
        void UpsertBudgetLibrary(BudgetLibraryDTO dto);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId);

        void DeleteBudgetLibrary(Guid libraryId);

        List<BudgetDTO> GetLibraryBudgets(Guid libraryId);

        BudgetLibraryDTO GetBudgetLibrary(Guid libraryId);

        List<BudgetDTO> GetScenarioBudgets(Guid simulationId);

        void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId);

        /// <summary>Returned dictionary values are the names of the corresponding scenario budgets.</summary>
        Dictionary<Guid, string> GetScenarioBudgetDictionary(List<Guid> budgetIds);
        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildren();
        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildrenAccessibleToUser(Guid userId);
        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid budgetLibraryId, IList<LibraryUserDTO> libraryUsers);
        List<LibraryUserDTO> GetLibraryUsers(Guid budgetLibraryId);

        DateTime GetLibraryModifiedDate(Guid budgetLibraryId);

        List<int> GetBudgetYearsBySimulationId(Guid simulationId);
        Dictionary<string, string> GetCriteriaPerBudgetNameForSimulation(Guid simulationId);
        Dictionary<string, string> GetCriteriaPerBudgetNameForBudgetLibrary(Guid budgetLibraryId);
        string GetBudgetLibraryName(Guid budgetLibraryId);
        void DeleteAllScenarioBudgetsForSimulation(Guid simulationId);
        void DeleteAllBudgetsForLibrary(Guid budgetLibraryId);
        void AddScenarioBudgets(Guid simulationId, List<BudgetDTO> newBudgetEntities);
        void UpdateScenarioBudgetAmounts(Guid simulationId, List<BudgetAmountDTOWithBudgetId> budgetAmounts);
        void AddScenarioBudgetAmounts(List<BudgetAmountDTOWithBudgetId> newBudgetAmountEntities);
        void AddLibraryBudgetAmounts(List<BudgetAmountDTOWithBudgetId> newBudgetAmountEntities);
        void AddBudgets(List<BudgetDTOWithLibraryId> budgets);
        void UpdateLibraryBudgetAmounts(List<BudgetAmountDTOWithBudgetId> budgetAmounts);
        void UpdateBudgetLibraryAndUpsertOrDeleteBudgets(BudgetLibraryDTO dto);
        void CreateNewBudgetLibrary(BudgetLibraryDTO dto, Guid userId);
        void UpsertOrDeleteScenarioBudgetsWithInvestmentPlan(List<BudgetDTO> budgets, InvestmentPlanDTO investmentPlan, Guid simulationId);
        void AddLibraryIdToScenarioBudget(List<BudgetDTO> budgetDTOs, Guid? libraryId);
        void AddModifiedToScenarioBudget(List<BudgetDTO> budgetDTOs, bool IsModified);
    }
}
