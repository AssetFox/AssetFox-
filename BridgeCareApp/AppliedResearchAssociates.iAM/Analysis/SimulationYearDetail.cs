﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        public double ConditionOfNetwork { get; set; }

        public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        public List<SectionDetail> Sections { get; } = new List<SectionDetail>();

        public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();

        public int Year { get; }
    }
}
