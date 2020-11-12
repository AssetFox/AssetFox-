using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentSchedulingEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public int OffsetToFutureYear { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
