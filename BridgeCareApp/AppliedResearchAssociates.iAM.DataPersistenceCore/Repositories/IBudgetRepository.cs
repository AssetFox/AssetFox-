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

        List<BudgetEntity> GetLibraryBudgets(Guid libraryId);

        BudgetLibraryDTO GetBudgetLibrary(Guid libraryId);

        List<BudgetDTO> GetScenarioBudgets(Guid simulationId);

        void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildren();
        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildrenAccessibleToUser(Guid userId);
        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid budgetLibraryId, IList<LibraryUserDTO> libraryUsers);
        List<LibraryUserDTO> GetLibraryUsers(Guid budgetLibraryId);
    }
}
