using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ICashFlowService
    {
        PagingPageModel<CashFlowRuleDTO> GetCashFlowPage(Guid simulationId, PagingRequestModel<CashFlowRuleDTO> request);
        PagingPageModel<CashFlowRuleDTO> GetLibraryCashFlowPage(Guid libraryId, PagingRequestModel<CashFlowRuleDTO> request);
        List<CashFlowRuleDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<CashFlowRuleDTO> request);
        List<CashFlowRuleDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<CashFlowRuleDTO> request);

        List<CashFlowRuleDTO> GetNewLibraryDataset(PagingSyncModel<CashFlowRuleDTO> pagingSync);
    }
}
