using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentLibraryUpsertPagingRequestModel : BaseLibraryUpsertPagingRequest<BudgetLibraryDTO>
    {
        public InvestmentLibraryUpsertPagingRequestModel()
        {
            SyncModel = new InvestmentPagingSyncModel();
        }
        public InvestmentPagingSyncModel SyncModel { get; set; }
    }
}
