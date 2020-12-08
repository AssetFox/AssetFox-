using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class EquationEntity
    {
        public Guid Id { get; set; }

        public string Expression { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }

        public virtual ConditionalTreatmentConsequenceEquationEntity ConditionalTreatmentConsequenceEquationJoin { get; set; }

        public virtual TreatmentCostEquationEntity TreatmentCostEquationJoin { get; set; }

        public virtual AttributeEquationCriterionLibraryEntity AttributeEquationCriterionLibraryJoin { get; set; }
    }
}
