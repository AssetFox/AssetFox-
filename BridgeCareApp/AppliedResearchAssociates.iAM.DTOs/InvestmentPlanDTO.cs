using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes the investment plan used for a simulation
    /// </summary>
    public class InvestmentPlanDTO : BaseDTO
    {
        /// <summary>
        /// The starting year for the simulation (plan year 1)
        /// </summary>
        public int FirstYearOfAnalysisPeriod { get; set; }

        /// <summary>
        /// Inflation rate to be used for the simulation, as a percentage.
        /// Valid range 0 - 100.
        /// </summary>
        public double InflationRatePercentage { get; set; }

        /// <summary>
        /// The minimum cost for any project selected.
        /// </summary>
        public decimal MinimumProjectCostLimit { get; set; }

        /// <summary>
        /// Number of planning years in the simulation
        /// </summary>
        public int NumberOfYearsInAnalysisPeriod { get; set; }

        /// <summary>
        /// Indicates that unused budgets should be moved to the next
        /// plan year.
        /// </summary>
        public bool ShouldAccumulateUnusedBudgetAmounts { get; set; }
    }
}
