using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IRemainingLifeLimitService
    {
        PagingPageModel<RemainingLifeLimitDTO> GetRemainingLifeLimitPage(Guid simulationId, PagingRequestModel<RemainingLifeLimitDTO> request);
        PagingPageModel<RemainingLifeLimitDTO> GetLibraryRemainingLifeLimitPage(Guid libraryId, PagingRequestModel<RemainingLifeLimitDTO> request);
        List<RemainingLifeLimitDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<RemainingLifeLimitDTO> request);
        List<RemainingLifeLimitDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<RemainingLifeLimitDTO> request);
    }
}
