using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ICashFlowPagingService
    {
        PagingPageModel<CashFlowRuleDTO> GetScenarioPage(Guid simulationId, PagingRequestModel<CashFlowRuleDTO> request);
        PagingPageModel<CashFlowRuleDTO> GetLibraryPage(Guid libraryId, PagingRequestModel<CashFlowRuleDTO> request);
        List<CashFlowRuleDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<CashFlowRuleDTO> request);
        List<CashFlowRuleDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO> request);
    }
}
