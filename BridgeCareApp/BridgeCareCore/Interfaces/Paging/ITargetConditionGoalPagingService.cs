using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ITargetConditionGoalPagingService
    {
        PagingPageModel<TargetConditionGoalDTO> GetTargetConditionGoalPage(Guid simulationId, PagingRequestModel<TargetConditionGoalDTO> request);
        PagingPageModel<TargetConditionGoalDTO> GetLibraryTargetConditionGoalPage(Guid libraryId, PagingRequestModel<TargetConditionGoalDTO> request);
        List<TargetConditionGoalDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TargetConditionGoalDTO> request);
        List<TargetConditionGoalDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<TargetConditionGoalDTO> request);
    }
}
