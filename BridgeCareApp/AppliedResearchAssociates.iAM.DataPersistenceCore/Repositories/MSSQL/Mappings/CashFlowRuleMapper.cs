using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class CashFlowRuleMapper
    {
        public static CashFlowRuleEntity ToEntity(this CashFlowRule domain, Guid cashFlowRuleLibraryId) =>
            new CashFlowRuleEntity
            {
                Id = Guid.NewGuid(),
                Name = domain.Name,
                CashFlowRuleLibraryId = cashFlowRuleLibraryId
            };

        public static CashFlowRule ToDomain(this CashFlowRuleEntity entity)
        {
            var cashFlowRule = new CashFlowRule(new Explorer()) {Name = entity.Name};

            if (entity.CriterionLibraryCashFlowRuleJoin != null)
            {
                cashFlowRule.Criterion.Expression =
                    entity.CriterionLibraryCashFlowRuleJoin.CriterionLibrary.MergedCriteriaExpression;
            }

            return cashFlowRule;
        }
    }
}
