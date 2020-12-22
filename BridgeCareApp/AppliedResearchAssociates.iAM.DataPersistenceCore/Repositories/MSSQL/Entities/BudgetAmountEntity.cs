using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetAmountEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid BudgetId { get; set; }

        public int Year { get; set; }

        public decimal Value { get; set; }

        public virtual BudgetEntity Budget { get; set; }
    }
}
