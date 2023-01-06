using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Interfaces
{
    public interface IBudgetPriortyPagingService : IPagingService<BudgetPriorityDTO, BudgetPriorityLibraryDTO>
    {
        PagingPageModel<BudgetPriorityDTO> GetBudgetPriortyPage(Guid simulationId, PagingRequestModel<BudgetPriorityDTO> request);
        PagingPageModel<BudgetPriorityDTO> GetLibraryBudgetPriortyPage(Guid libraryId, PagingRequestModel<BudgetPriorityDTO> request);
        List<BudgetPriorityDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<BudgetPriorityDTO> request);
        List<BudgetPriorityDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<BudgetPriorityDTO> request);

        List<BudgetPriorityDTO> GetNewLibraryDataset(PagingSyncModel<BudgetPriorityDTO> pagingSync);
    }
}
