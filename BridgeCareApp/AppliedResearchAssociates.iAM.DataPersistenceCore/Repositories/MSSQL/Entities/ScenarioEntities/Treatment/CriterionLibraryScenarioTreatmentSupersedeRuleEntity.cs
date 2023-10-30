using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class CriterionLibraryScenarioTreatmentSupersedeRuleEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioTreatmentSupersedeRuleId { get; set; }

        public virtual ScenarioTreatmentSupersedeRuleEntity ScenarioTreatmentSupersedeRule { get; set; }
    }
}
