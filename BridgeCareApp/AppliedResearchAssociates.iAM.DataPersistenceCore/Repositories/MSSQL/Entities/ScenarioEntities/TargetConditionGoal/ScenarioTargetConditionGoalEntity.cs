using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class ScenarioTargetConditionGoalEntity : ConditionGoalEntity
    {
        //public Guid ScenarioTargetConditionGoalId { get; set; }

        public Guid SimulationId { get; set; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public virtual SimulationEntity Simulation { get; set;}

        public virtual CriterionLibraryScenarioTargetConditionGoalEntity CriterionLibraryScenarioTargetConditionGoalJoin { get; set; }
    }
}
