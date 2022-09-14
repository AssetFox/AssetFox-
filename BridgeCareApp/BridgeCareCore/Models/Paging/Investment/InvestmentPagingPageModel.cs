using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingPageModel : PagingPageModel<BudgetDTO>
    {
        public InvestmentPlanDTO Investment { get; set; }
        public decimal LastYear { get; set; }
    }
}
