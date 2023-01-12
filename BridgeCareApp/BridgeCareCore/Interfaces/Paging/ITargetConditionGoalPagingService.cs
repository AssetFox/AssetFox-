using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ITargetConditionGoalPagingService
    {
        PagingPageModel<TargetConditionGoalDTO> GetScenarioPage(Guid simulationId, PagingRequestModel<TargetConditionGoalDTO> request);
        PagingPageModel<TargetConditionGoalDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<TargetConditionGoalDTO> request);
        List<TargetConditionGoalDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<TargetConditionGoalDTO> request);
        List<TargetConditionGoalDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO> upsertRequest);
    }
}
