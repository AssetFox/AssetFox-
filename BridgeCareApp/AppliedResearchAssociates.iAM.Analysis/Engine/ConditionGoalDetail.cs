﻿namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public abstract class ConditionGoalDetail
    {
        public string AttributeName { get; set; }

        public bool GoalIsMet { get; set; }

        public string GoalName { get; set; }
    }
}