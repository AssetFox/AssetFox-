using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryCashFlowRuleEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid CashFlowRuleId { get; set; }

        public virtual CashFlowRuleEntity CashFlowRule { get; set; }
    }
}
