using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryBudgetConditionEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid BudgetConditionId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual BudgetConditionEntity BudgetCondition { get; set; }
    }
}
