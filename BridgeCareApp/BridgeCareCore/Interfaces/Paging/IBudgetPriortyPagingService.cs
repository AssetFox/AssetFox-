using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Interfaces
{
    public interface IBudgetPriortyPagingService 
    {
        PagingPageModel<BudgetPriorityDTO> GetScenarioPage(Guid simulationId, PagingRequestModel<BudgetPriorityDTO> request);
        PagingPageModel<BudgetPriorityDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<BudgetPriorityDTO> request);
        List<BudgetPriorityDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<BudgetPriorityDTO> request);
        List<BudgetPriorityDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO> request);
    }
}
