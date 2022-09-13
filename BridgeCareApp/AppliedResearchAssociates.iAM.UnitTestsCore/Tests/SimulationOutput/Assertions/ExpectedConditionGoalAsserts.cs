using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class DeficientConditionGoalDetailAsserts
    {
        public static void Same(DeficientConditionGoalDetail expected, DeficientConditionGoalDetail actual)
        {
            Assert.Equal(expected.DeficientLimit, actual.DeficientLimit);
            Assert.Equal(expected.ActualDeficientPercentage, actual.ActualDeficientPercentage);
            Assert.Equal(expected.AllowedDeficientPercentage, actual.AllowedDeficientPercentage);
            Assert.Equal(expected.GoalName, actual.GoalName);
            Assert.Equal(expected.AttributeName, actual.AttributeName);
        }
    }
}
