using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibrarySelectableTreatmentEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid SelectableTreatmentId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
