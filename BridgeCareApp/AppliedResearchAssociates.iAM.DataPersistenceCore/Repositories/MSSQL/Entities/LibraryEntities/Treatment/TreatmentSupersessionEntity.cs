using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentSupersessionEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public virtual CriterionLibraryTreatmentSupersessionEntity CriterionLibraryTreatmentSupersessionJoin { get; set; }
    }
}
