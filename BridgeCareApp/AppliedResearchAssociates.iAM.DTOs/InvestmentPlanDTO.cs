using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class InvestmentPlanDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public int FirstYearOfAnalysisPeriod { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public double InflationRatePercentage { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public decimal MinimumProjectCostLimit { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int NumberOfYearsInAnalysisPeriod { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool ShouldAccumulateUnusedBudgetAmounts { get; set; }
    }
}
