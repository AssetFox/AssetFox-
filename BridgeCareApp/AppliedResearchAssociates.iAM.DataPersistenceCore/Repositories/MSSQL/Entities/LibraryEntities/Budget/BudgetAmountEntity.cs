using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget
{
    public class BudgetAmountEntity : BaseBudgetAmountEntity
    {
        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }
    }
}
