using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule
{
    public static class CashFlowRuleLibraryDtos
    {
        public static CashFlowRuleLibraryDTO Empty()
        {
            var library = new CashFlowRuleLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "TestCashFlowRuleLibrary",
                CashFlowRules = new List<CashFlowRuleDTO>(),
            };
            return library;
        }
        public static CashFlowRuleLibraryDTO WithSingleRule()
        {
            var library = Empty();
            var rule = CashFlowRuleDtos.Rule();
            library.CashFlowRules.Add(rule);
            return library;
        }
    }
}
