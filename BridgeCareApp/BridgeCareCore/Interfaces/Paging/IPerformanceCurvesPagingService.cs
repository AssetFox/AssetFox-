using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IPerformanceCurvesPagingService
    {
        PagingPageModel<PerformanceCurveDTO> GetScenarioPage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> request);
        PagingPageModel<PerformanceCurveDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO> upsertRequest);
    }
}
