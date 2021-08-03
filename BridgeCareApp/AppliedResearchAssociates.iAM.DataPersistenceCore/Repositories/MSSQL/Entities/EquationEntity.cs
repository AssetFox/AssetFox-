using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class EquationEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Expression { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }

        public virtual ConditionalTreatmentConsequenceEquationEntity ConditionalTreatmentConsequenceEquationJoin { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEquationEntity ScenarioConditionalTreatmentConsequenceEquationJoin { get; set; }

        public virtual TreatmentCostEquationEntity TreatmentCostEquationJoin { get; set; }

        public virtual ScenarioTreatmentCostEquationEntity ScenarioTreatmentCostEquationJoin { get; set; }

        public virtual AttributeEquationCriterionLibraryEntity AttributeEquationCriterionLibraryJoin { get; set; }

        public virtual BenefitQuantifierEntity BenefitQuantifier { get; set; }
    }
}
