using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryBudgetPriorityEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid BudgetPriorityId { get; set; }

        public virtual BudgetPriorityEntity BudgetPriority { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
