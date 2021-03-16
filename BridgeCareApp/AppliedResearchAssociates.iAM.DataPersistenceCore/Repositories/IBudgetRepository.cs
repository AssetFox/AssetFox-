using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetRepository
    {
        void CreateBudgetLibrary(string name, Guid simulationId);

        void CreateBudgets(List<Budget> budgets, Guid simulationId);

        Task<List<SimpleBudgetDetailDTO>> ScenarioSimpleBudgetDetails(Guid simulationId);

        Task<List<BudgetLibraryDTO>> BudgetLibrariesWithBudgets();

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, BudgetLibraryDTO dto);

        void UpsertBudgetLibrary(BudgetLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId, UserInfoDTO userInfo);

        void DeleteBudgetLibrary(Guid libraryId);
    }
}
