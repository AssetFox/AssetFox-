using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TargetConditionGoalDetails
    {
        public static TargetConditionGoalDetail Detail(string attributeName)
        {
            var detail = new TargetConditionGoalDetail
            {
                ActualValue = 10,
                AttributeName = attributeName,
                GoalIsMet = true,
                GoalName = "Goal",
                TargetValue = 20,
            };
            return detail;
        }
    }
}
