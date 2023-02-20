using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule
{
    public static class CashFlowRuleDtos
    {
        public static CashFlowRuleDTO Rule()
        {
            var rule = new CashFlowRuleDTO
            {
                Name = "TestCashFlowRule",
                Id = Guid.NewGuid(),
                CashFlowDistributionRules = new List<CashFlowDistributionRuleDTO>
                {
                    CashFlowDistributionRuleDtos.Dto(),
                },
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Name = "TestCriterionLibrary",
                    Id = Guid.NewGuid(),
                    IsSingleUse = true,
                },
            };
            return rule;
        }
    }
}
