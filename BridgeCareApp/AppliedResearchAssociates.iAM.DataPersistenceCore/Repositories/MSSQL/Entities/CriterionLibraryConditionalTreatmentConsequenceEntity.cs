using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryConditionalTreatmentConsequenceEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid ConditionalTreatmentConsequenceId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual ConditionalTreatmentConsequenceEntity ConditionalTreatmentConsequence { get; set; }
    }
}
