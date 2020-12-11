using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class CashFlowDistributionRuleMapper
    {
        public static CashFlowDistributionRuleEntity ToEntity(this CashFlowDistributionRule domain, Guid cashFlowRuleId, int durationInYears) =>
            new CashFlowDistributionRuleEntity
            {
                Id = domain.Id,
                CashFlowRuleId = cashFlowRuleId,
                DurationInYears = durationInYears,
                YearlyPercentages = string.Join('/', domain.YearlyPercentages),
                CostCeiling = domain.CostCeiling ?? 0
            };
    }
}
