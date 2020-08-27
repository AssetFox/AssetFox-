﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationStatusDetail
    {
        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        public double ConditionOfNetwork { get; set; }

        public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();
    }
}
