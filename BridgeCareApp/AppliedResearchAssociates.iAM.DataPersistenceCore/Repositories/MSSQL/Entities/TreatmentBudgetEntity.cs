using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentBudgetEntity
    {
        public Guid TreatmentId { get; set; }
        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }
        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
