using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTreatmentConsequenceEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentConsequenceId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual TreatmentConsequenceEntity TreatmentConsequence { get; set; }
    }
}
