﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public class BudgetPriority
    {
        public List<BudgetPercentage> BudgetPercentages { get; }

        public Criterion Criterion { get; }

        public int PriorityLevel { get; } // Isn't this just a strict linear ordering? Or can multiple priorities have the same priority level?

        public int YearOfAnalysisPeriod { get; }
    }
}
