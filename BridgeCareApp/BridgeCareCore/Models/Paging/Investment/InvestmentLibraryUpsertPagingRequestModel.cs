using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentLibraryUpsertPagingRequestModel : BaseLibraryUpsertPagingRequest<BudgetLibraryDTO>
    {
        public InvestmentLibraryUpsertPagingRequestModel()
        {
            PagingSync = new InvestmentPagingSyncModel();
        }
        public InvestmentPagingSyncModel PagingSync { get; set; }
    }
}
