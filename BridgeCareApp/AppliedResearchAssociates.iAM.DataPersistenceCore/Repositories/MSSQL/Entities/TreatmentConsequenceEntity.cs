using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentConsequenceEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public Guid AttributeId { get; set; }

        public string ChangeValue { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public virtual CriterionLibraryTreatmentConsequenceEntity CriterionLibraryTreatmentConsequenceJoin { get; set; }

        public virtual TreatmentConsequenceEquationEntity TreatmentConsequenceEquationJoin { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
