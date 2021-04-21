using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CashFlowDistributionRuleDTO : BaseDTO
    {
        public int DurationInYears { get; set; }

        public decimal CostCeiling { get; set; }

        public string YearlyPercentages { get; set; }
    }
}
