using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class AnalysisMethod
    {
        public bool AllowFundingFromMultipleBudgets { get; set; }

        public string BenefitAttributeName { get; set; }

        public double BenefitLimit { get; set; }

        public string BenefitWeightAttributeName { get; set; }

        public List<BudgetPriority> BudgetPriorities { get; set; }

        public List<DeficientConditionGoal> DeficientConditionGoals { get; set; }

        public string FilterExpression { get; set; }

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public List<RemainingLifeLimit> RemainingLifeLimits { get; set; }

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public bool ShouldDeteriorateDuringCashFlow { get; set; }

        public bool ShouldRestrictCashFlowToFirstYearBudgets { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }

        public List<TargetConditionGoal> TargetConditionGoals { get; set; }
    }
}
