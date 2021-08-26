using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class CriterionLibraryScenarioConditionalTreatmentConsequenceEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioConditionalTreatmentConsequenceId { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEntity ScenarioConditionalTreatmentConsequence { get; set; }
    }
}
