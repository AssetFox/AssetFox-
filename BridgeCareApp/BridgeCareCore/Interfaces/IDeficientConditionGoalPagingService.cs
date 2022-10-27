using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IDeficientConditionGoalPagingService
    {
        PagingPageModel<DeficientConditionGoalDTO> GetScenarioDeficientConditionGoalPage(Guid simulationId, PagingRequestModel<DeficientConditionGoalDTO> request);
        PagingPageModel<DeficientConditionGoalDTO> GetLibraryDeficientConditionGoalPage(Guid libraryId, PagingRequestModel<DeficientConditionGoalDTO> request);
        List<DeficientConditionGoalDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<DeficientConditionGoalDTO> request);
        List<DeficientConditionGoalDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<DeficientConditionGoalDTO> request);
    }
}
