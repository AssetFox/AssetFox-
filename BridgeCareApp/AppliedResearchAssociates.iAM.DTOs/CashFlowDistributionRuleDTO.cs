using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class CashFlowDistributionRuleDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public int DurationInYears { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public decimal CostCeiling { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string YearlyPercentages { get; set; }
    }
}
