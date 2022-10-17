using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repsitories;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetRepository
    {
        void CreateScenarioBudgets(List<Budget> budgets, Guid simulationId);

        List<SimpleBudgetDetailDTO> GetScenarioSimpleBudgetDetails(Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibraries();

        /// <summary>As of October, 2022, it is expected that the person who created the library will be an owner,
        /// and no one else will be an owner. However, this is NOT enforced at the repository level. The repository
        /// will blindly update the users to whatever the controller asks for. Therefore, it is the responsibility
        /// of the controller to make sure the users are correct.</summary>
        void UpsertBudgetLibrary(BudgetLibraryDTO dto);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId);

        void DeleteBudgetLibrary(Guid libraryId);

        List<BudgetEntity> GetLibraryBudgets(Guid libraryId);

        BudgetLibraryDTO GetBudgetLibrary(Guid libraryId);

        List<BudgetDTO> GetScenarioBudgets(Guid simulationId);

        void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId);

        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildren();
        List<BudgetLibraryDTO> GetBudgetLibrariesNoChildrenAccessibleToUser(Guid userId);
        LibraryAccessModel GetLibraryAccess(Guid libraryId, Guid userId);
    }
}
