using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow
{
    public class CashFlowDistributionRuleEntity : BaseCashFlowDistributionRuleEntity
    {
        public Guid CashFlowRuleId { get; set; }

        public virtual CashFlowRuleEntity CashFlowRule { get; set; }
    }
}
