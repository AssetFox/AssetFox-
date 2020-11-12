using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPercentagePairEntity
    {
        public Guid Id { get; set; }
        public Guid BudgetId { get; set; }
        public Guid BudgetPriorityId { get; set; }

        public virtual BudgetEntity Budget { get; set; }
        public virtual BudgetPriorityEntity BudgetPriority { get; set; }
    }
}
