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
                Id = domain.Id,
                Name = domain.Name,
                CashFlowRuleLibraryId = cashFlowRuleLibraryId
            };
    }
}
