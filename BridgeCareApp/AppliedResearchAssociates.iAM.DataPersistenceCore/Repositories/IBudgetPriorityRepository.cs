using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPriorityRepository
    {
        void CreateBudgetPriorityLibrary(string name, Guid simulationId);

        void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId);

        List<BudgetPriorityLibraryDTO> BudgetPriorityLibrariesWithBudgetPriorities();

        void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId);

        void DeleteBudgetPriorityLibrary(Guid libraryId);
    }
}
