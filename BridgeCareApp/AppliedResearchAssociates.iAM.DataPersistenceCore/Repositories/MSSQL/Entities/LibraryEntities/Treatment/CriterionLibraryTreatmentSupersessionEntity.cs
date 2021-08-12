using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class CriterionLibraryTreatmentSupersessionEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentSupersessionId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual TreatmentSupersessionEntity TreatmentSupersession { get; set; }
    }
}
