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
        public static CashFlowRuleDTO Rule(Guid? id = null, Guid? distributionRuleId = null, Guid? criterionLibraryId = null)
        {
            var ruleId = id ?? Guid.NewGuid();
            var resolveLibraryId = criterionLibraryId ?? Guid.NewGuid();
            var rule = new CashFlowRuleDTO
            {
                Name = "TestCashFlowRule",
                Id = ruleId,
                CashFlowDistributionRules = new List<CashFlowDistributionRuleDTO>
                {
                    CashFlowDistributionRuleDtos.Dto(distributionRuleId),
                },
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Name = "TestCriterionLibrary",
                    MergedCriteriaExpression = "mergedCriteriaExpression",
                    Id = resolveLibraryId,
                    IsSingleUse = true,
                },
            };
            return rule;
        }
    }
}
