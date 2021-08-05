using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget
{
    public class BudgetLibraryEntity : LibraryEntity
    {
        public BudgetLibraryEntity() => Budgets = new HashSet<BudgetEntity>();

        public virtual ICollection<BudgetEntity> Budgets { get; set; }
    }
}
