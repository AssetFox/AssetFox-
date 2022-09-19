using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CashFlowConsiderationDetails
    {
        public static CashFlowConsiderationDetail Detail()
        {
            var detail = new CashFlowConsiderationDetail("CashFlowRuleName")
            {
                ReasonAgainstCashFlow = ReasonAgainstCashFlow.Undefined,
            };
            return detail;
        }
    }
}
