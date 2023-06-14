using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines a specific cash flow rule
    /// </summary>
    public class CashFlowDistributionRuleDTO : BaseDTO
    {
        /// <summary>
        /// How many years will share project costs 
        /// </summary>
        public int DurationInYears { get; set; }

        /// <summary>
        /// The maximum project value that can use this rule
        /// </summary>
        public decimal CostCeiling { get; set; }

        /// <summary>
        /// A string that defines what percentage of the project cost will be spent
        /// each year.  Format is Y1/Y2/Y3.  The number of years specified should
        /// match the DurationInYears
        /// </summary>
        public string YearlyPercentages { get; set; }
    }
}
