using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class CriterionLibraryScenarioTreatmentSupersessionEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid TreatmentSupersessionId { get; set; }

        public virtual ScenarioTreatmentSupersessionEntity ScenarioTreatmentSupersession { get; set; }
    }
}
