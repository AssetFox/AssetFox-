using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetAmountEntity
    {
        public Guid Id { get; set; }
        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }
    }
}
