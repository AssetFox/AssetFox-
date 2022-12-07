using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingPageModel : PagingPageModel<BudgetDTO>
    {
        public InvestmentPlanDTO InvestmentPlan { get; set; }
        public int LastYear { get; set; }
        public int FirstYear { get; set; }
    }
}
