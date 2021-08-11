using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class CriterionLibraryConditionalTreatmentConsequenceEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid ConditionalTreatmentConsequenceId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual ConditionalTreatmentConsequenceEntity ConditionalTreatmentConsequence { get; set; }
    }
}
