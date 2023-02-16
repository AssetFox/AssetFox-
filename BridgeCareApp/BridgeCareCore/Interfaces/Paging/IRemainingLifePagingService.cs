using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using BridgeCareCore.Services.Paging.Generics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BridgeCareCore.Interfaces
{
    public interface IRemainingLifeLimitPagingService 
    {
        PagingPageModel<RemainingLifeLimitDTO> GetScenarioPage(Guid scenarioId, PagingRequestModel<RemainingLifeLimitDTO> request);
        PagingPageModel<RemainingLifeLimitDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<RemainingLifeLimitDTO> request);
        List<RemainingLifeLimitDTO> GetSyncedScenarioDataSet(Guid scenarioId, PagingSyncModel<RemainingLifeLimitDTO> syncModel);
        List<RemainingLifeLimitDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO> upsertRequest);
    }
}
