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
        void AddOrUpdateBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId);
        void AddOrUpdateOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId);
        void DeleteBudgetPriorityLibrary(Guid libraryId);
    }
}
