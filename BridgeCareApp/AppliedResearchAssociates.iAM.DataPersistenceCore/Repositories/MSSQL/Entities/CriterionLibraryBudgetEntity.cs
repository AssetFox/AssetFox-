using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryBudgetEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid BudgetId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual BudgetEntity Budget { get; set; }
    }
}
