using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTreatmentSupersessionEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentSupersessionId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual TreatmentSupersessionEntity TreatmentSupersession { get; set; }
    }
}
