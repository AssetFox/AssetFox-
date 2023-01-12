using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IInvestmentPagingService
    {
        InvestmentPagingPageModel GetLibraryPage(Guid libraryId, InvestmentPagingRequestModel request);

        InvestmentPagingPageModel GetScenarioPage(Guid simulationId, InvestmentPagingRequestModel request);

        List<BudgetDTO> GetSyncedScenarioDataSet(Guid simulationId, InvestmentPagingSyncModel request);

        List<BudgetDTO> GetSyncedLibraryDataset(Guid libraryId, InvestmentPagingSyncModel request);
        List<BudgetDTO> GetNewLibraryDataset(InvestmentPagingSyncModel request);
    }
}
