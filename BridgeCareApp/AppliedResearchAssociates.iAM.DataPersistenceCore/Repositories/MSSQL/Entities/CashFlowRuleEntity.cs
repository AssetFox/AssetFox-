using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowRuleEntity
    {
        public CashFlowRuleEntity()
        {
            CashFlowDistributionRules = new HashSet<CashFlowDistributionRuleEntity>();
        }

        public Guid Id { get; set; }
        public Guid CashFlowRuleLibraryId { get; set; }

        public virtual CashFlowRuleLibraryEntity CashFlowRuleLibrary { get; set; }
        public virtual CriterionLibraryCashFlowRuleEntity CriterionLibraryCashFlowRuleJoin { get; set; }
        public virtual ICollection<CashFlowDistributionRuleEntity> CashFlowDistributionRules { get; set; }
    }
}
