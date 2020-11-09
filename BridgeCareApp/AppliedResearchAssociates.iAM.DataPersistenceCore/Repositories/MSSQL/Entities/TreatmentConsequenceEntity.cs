using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentConsequenceEntity
    {
        public Guid Id { get; set; }
        public Guid TreatmentId { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
        public virtual CriterionLibraryTreatmentConsequence CriterionLibraryTreatmentConsequenceJoin { get; set; }
        public virtual TreatmentConsequenceEquationEntity TreatmentConsequenceEquationJoin { get; set; }
    }
}
