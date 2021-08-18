using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority
{
    public class BudgetPriorityEntity : BaseBudgetPriorityEntity
    {
        public Guid BudgetPriorityLibraryId { get; set; }

        public virtual BudgetPriorityLibraryEntity BudgetPriorityLibrary { get; set; }

        public virtual CriterionLibraryBudgetPriorityEntity CriterionLibraryBudgetPriorityJoin { get; set; }
    }
}
