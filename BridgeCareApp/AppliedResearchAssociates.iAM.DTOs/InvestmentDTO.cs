using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class InvestmentDTO
    {
        public InvestmentPlanDTO InvestmentPlan { get; set; }

        public List<BudgetDTO> ScenarioBudgets { get; set; }
    }
}
