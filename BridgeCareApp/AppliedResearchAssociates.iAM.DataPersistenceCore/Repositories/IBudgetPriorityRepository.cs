using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPriorityRepository
    {
        void CreateBudgetPriorityLibrary(string name, Guid simulationId);

        void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId);

        Task<List<BudgetPriorityLibraryDTO>> BudgetPriorityLibrariesWithBudgetPriorities();

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, BudgetPriorityLibraryDTO dto);

        void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId, UserInfoDTO userInfo);

        void DeleteBudgetPriorityLibrary(Guid libraryId);
    }
}
