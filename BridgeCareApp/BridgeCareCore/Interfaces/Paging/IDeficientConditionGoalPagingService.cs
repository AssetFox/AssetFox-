using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IDeficientConditionGoalPagingService
    {
        PagingPageModel<DeficientConditionGoalDTO> GetScenarioPage(Guid simulationId, PagingRequestModel<DeficientConditionGoalDTO> request);
        PagingPageModel<DeficientConditionGoalDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<DeficientConditionGoalDTO> request);
        List<DeficientConditionGoalDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<DeficientConditionGoalDTO> request);
        List<DeficientConditionGoalDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO> upsertRequest);
    }
}
