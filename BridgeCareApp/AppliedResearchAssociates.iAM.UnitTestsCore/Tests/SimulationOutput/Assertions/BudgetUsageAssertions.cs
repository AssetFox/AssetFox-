using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetUsageAssertions
    {
        public static void Same(BudgetUsageDetail expected, BudgetUsageDetail actual)
        {
            Assert.Equal(expected.BudgetName, actual.BudgetName);
            DecimalAssertions.ApproximatelyEqual(expected.CoveredCost, actual.CoveredCost);
            Assert.Equal(expected.Status, actual.Status);
        }
    }
}
