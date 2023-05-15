using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        /// <summary>
        /// .
        /// </summary>
        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        /// <summary>
        /// .
        /// </summary>
        public double ConditionOfNetwork { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        /// <summary>
        /// .
        /// </summary>
        public List<AssetDetail> Assets { get; } = new List<AssetDetail>();

        /// <summary>
        /// .
        /// </summary>
        public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();

        /// <summary>
        /// .
        /// </summary>
        public int Year { get; }
    }
}
