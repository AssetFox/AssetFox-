using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingRequestModel : PagingRequestModel<BudgetDTO>
    {
        public new InvestmentPagingSyncModel SyncModel { get; set; }
    }
}
