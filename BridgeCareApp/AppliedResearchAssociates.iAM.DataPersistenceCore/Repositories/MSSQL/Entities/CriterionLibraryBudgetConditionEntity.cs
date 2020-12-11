using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryBudgetConditionEntity
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid BudgetConditionId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual BudgetConditionEntity BudgetCondition { get; set; }
    }
}
