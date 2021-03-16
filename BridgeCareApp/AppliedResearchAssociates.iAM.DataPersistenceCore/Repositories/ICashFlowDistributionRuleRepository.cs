using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowDistributionRuleRepository
    {
        void CreateCashFlowDistributionRules(
            Dictionary<Guid, List<CashFlowDistributionRule>> distributionRulesPerCashFlowRuleEntityId);

        void UpsertOrDeleteCashFlowDistributionRules(
            Dictionary<Guid, List<CashFlowDistributionRuleDTO>> distributionRulesPerCashFlowRuleId, Guid libraryId, Guid? userId = null);
    }
}
