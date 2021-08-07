using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class CriterionLibraryScenarioTreatmentSupersessionEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid TreatmentSupersessionId { get; set; }

        public virtual ScenarioTreatmentSupersessionEntity ScenarioTreatmentSupersession { get; set; }
    }
}
