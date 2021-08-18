using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority
{
    public class CriterionLibraryBudgetPriorityEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid BudgetPriorityId { get; set; }

        public virtual BudgetPriorityEntity BudgetPriority { get; set; }
    }
}
