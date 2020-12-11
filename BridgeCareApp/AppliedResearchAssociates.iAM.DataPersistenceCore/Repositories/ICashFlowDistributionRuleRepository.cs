using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowDistributionRuleRepository
    {
        void CreateCashFlowDistributionRules(Dictionary<Guid, List<CashFlowDistributionRule>> distributionRulesPerCashFlowRuleEntityId);
    }
}
