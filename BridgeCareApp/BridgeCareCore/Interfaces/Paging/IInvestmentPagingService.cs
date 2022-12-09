using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IInvestmentPagingService
    {
        InvestmentPagingPageModel GetLibraryInvestmentPage(Guid libraryId, InvestmentPagingRequestModel request);

        InvestmentPagingPageModel GetScenarioInvestmentPage(Guid simulationId, InvestmentPagingRequestModel request);

        List<BudgetDTO> GetSyncedInvestmentDataset(Guid simulationId, InvestmentPagingSyncModel request);

        List<BudgetDTO> GetSyncedLibraryDataset(Guid libraryId, InvestmentPagingSyncModel request);
    }
}
