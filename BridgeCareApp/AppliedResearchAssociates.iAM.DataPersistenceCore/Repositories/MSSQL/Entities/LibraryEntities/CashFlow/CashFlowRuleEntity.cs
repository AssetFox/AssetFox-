using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowRuleEntity : BaseCashFlowRuleEntity
    {
        public CashFlowRuleEntity()
        {
            CashFlowDistributionRules = new HashSet<CashFlowDistributionRuleEntity>();
        }

        public Guid CashFlowRuleLibraryId { get; set; }

        public virtual CashFlowRuleLibraryEntity CashFlowRuleLibrary { get; set; }

        public virtual CriterionLibraryCashFlowRuleEntity CriterionLibraryCashFlowRuleJoin { get; set; }

        public virtual ICollection<CashFlowDistributionRuleEntity> CashFlowDistributionRules { get; set; }
    }
}
