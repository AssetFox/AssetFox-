using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetPercentagePairDTO : BaseDTO
    {
        public decimal Percentage { get; set; }

        public Guid BudgetId { get; set; }

        public string BudgetName { get; set; }
    }
}
