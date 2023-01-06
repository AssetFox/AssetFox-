using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IPerformanceCurvesPagingService
    {
        PagingPageModel<PerformanceCurveDTO> GetScenarioPerformanceCurvePage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> request);
        PagingPageModel<PerformanceCurveDTO> GetLibraryPerformanceCurvePage(Guid libraryId, PagingRequestModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetNewLibraryDataset(PagingSyncModel<PerformanceCurveDTO> pagingSync);
    }
}
