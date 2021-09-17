using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class EquationEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Expression { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }

        public virtual ScenarioPerformanceCurveEquationEntity ScenarioPerformanceCurveEquationJoin { get; set; }

        public virtual EquationCalculatedAttributePairEntity CalculatedAttributePairJoin { get; set; }

        public virtual ScenarioEquationCalculatedAttributePairEntity ScenarioCalculatedAttributePairJoin { get; set; }

        public virtual ConditionalTreatmentConsequenceEquationEntity ConditionalTreatmentConsequenceEquationJoin { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEquationEntity ScenarioConditionalTreatmentConsequenceEquationJoin { get; set; }

        public virtual TreatmentCostEquationEntity TreatmentCostEquationJoin { get; set; }

        public virtual ScenarioTreatmentCostEquationEntity ScenarioTreatmentCostEquationJoin { get; set; }

        public virtual AttributeEquationCriterionLibraryEntity AttributeEquationCriterionLibraryJoin { get; set; }

        public virtual BenefitQuantifierEntity BenefitQuantifier { get; set; }
    }
}
