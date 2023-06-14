using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// Results for a given time period in a scenario
    /// </summary>
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        /// <summary>
        /// List of budgets available for this time period
        /// </summary>
        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        /// <summary>
        /// Average value of the benefit attribute for this time period
        /// </summary>
        public double ConditionOfNetwork { get; set; }

        /// <summary>
        /// List of the maximum deficiencies allowed during this time period
        /// </summary>
        public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        /// <summary>
        /// The state of each asset in the simulation at the END of this time period
        /// </summary>
        public List<AssetDetail> Assets { get; } = new List<AssetDetail>();

        /// <summary>
        /// List of the average target conditions the simulation is trying to achieve
        /// in this time period.
        /// </summary>
        public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();

        /// <summary>
        /// The year (or other time period) of this result
        /// </summary>
        public int Year { get; }
    }
}
