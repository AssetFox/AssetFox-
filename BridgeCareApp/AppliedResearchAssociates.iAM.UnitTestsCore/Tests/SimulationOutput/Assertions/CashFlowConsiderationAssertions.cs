using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CashFlowConsiderationAssertions
    {
        public static void Same(CashFlowConsiderationDetail expected, CashFlowConsiderationDetail actual)
        {
            Assert.Equal(expected.CashFlowRuleName, actual.CashFlowRuleName);
            Assert.Equal(expected.ReasonAgainstCashFlow, actual.ReasonAgainstCashFlow);
        }
    }
}
