using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class ScenarioConditionalTreatmentConsequenceEntity : TreatmentConsequenceEntity
    {
        public Guid ScenarioSelectableTreatmentId { get; set; }

        public virtual ScenarioSelectableTreatmentEntity ScenarioSelectableTreatment { get; set; }

        public virtual CriterionLibraryScenarioConditionalTreatmentConsequenceEntity CriterionLibraryScenarioConditionalTreatmentConsequenceJoin { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEquationEntity ScenarioConditionalTreatmentConsequenceEquationJoin { get; set; }
    }
}
