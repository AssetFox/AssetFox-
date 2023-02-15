using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingRequestModel : BasePagingRequest
    {
        public InvestmentPagingSyncModel SyncModel { get; set; }
    }
}
