using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class ScenarioSelectableTreatmentBudgetEntity : BaseEntity
    {
        public Guid ScenarioSelectableTreatmentId { get; set; }

        public Guid BudgetId { get; set; }

        public virtual BudgetEntity Budget { get; set; }

        public virtual ScenarioSelectableTreatmentEntity ScenarioSelectableTreatment { get; set; }
    }
}
