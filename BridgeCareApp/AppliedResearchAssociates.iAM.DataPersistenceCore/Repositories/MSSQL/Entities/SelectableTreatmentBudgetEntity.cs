using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SelectableTreatmentBudgetEntity
    {
        public Guid SelectableTreatmentId { get; set; }

        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
