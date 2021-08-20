﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal
{
    public class ScenarioTargetConditionGoalEntity : ConditionGoalEntity
    {
        public Guid SimulationId { get; set; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public virtual SimulationEntity Simulation { get; set;}

        public virtual CriterionLibraryScenarioTargetConditionGoalEntity CriterionLibraryScenarioTargetConditionGoalJoin { get; set; }
    }
}
