using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryCashFlowRuleEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid CashFlowRuleId { get; set; }

        public virtual CashFlowRuleEntity CashFlowRule { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
