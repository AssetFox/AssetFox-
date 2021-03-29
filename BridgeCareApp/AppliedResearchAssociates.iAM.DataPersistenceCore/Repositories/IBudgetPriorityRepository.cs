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

        List<BudgetPriorityLibraryDTO> BudgetPriorityLibrariesWithBudgetPriorities();

        void UpsertPermitted(Guid simulationId, BudgetPriorityLibraryDTO dto);

        void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId);

        void DeleteBudgetPriorityLibrary(Guid libraryId);
    }
}
