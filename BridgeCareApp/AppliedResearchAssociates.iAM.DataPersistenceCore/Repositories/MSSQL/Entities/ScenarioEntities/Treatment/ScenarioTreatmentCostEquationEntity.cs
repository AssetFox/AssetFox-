using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class ScenarioTreatmentCostEquationEntity : BaseEquationJoinEntity
    {
        public Guid ScenarioTreatmentCostId { get; set; }
        public virtual ScenarioTreatmentCostEntity ScenarioTreatmentCost { get; set; }
    }
}
