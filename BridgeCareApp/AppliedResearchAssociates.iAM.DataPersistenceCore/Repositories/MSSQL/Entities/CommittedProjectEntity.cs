using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CommittedProjectEntity : TreatmentEntity
    {
        public Guid BudgetId { get; set; }

        public Guid SelectableTreatmentId { get; set; }

        public Guid SectionId { get; set; }

        public double Cost { get; set; }

        public int Year { get; set; }

        public virtual BudgetEntity Budget { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public virtual SectionEntity Section { get; set; }
    }
}
