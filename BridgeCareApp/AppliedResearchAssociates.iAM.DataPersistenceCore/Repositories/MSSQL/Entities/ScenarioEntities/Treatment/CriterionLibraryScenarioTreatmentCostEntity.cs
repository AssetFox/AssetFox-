using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class CriterionLibraryScenarioTreatmentCostEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioTreatmentCostId { get; set; }
        public virtual ScenarioTreatmentCostEntity ScenarioTreatmentCost { get; set; }
    }
}
