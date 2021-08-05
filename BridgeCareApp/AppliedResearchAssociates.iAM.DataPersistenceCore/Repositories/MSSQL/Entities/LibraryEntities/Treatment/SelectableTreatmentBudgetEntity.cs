using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SelectableTreatmentBudgetEntity : BaseEntity
    {
        public Guid SelectableTreatmentId { get; set; }

        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
