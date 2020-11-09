using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DeficientConditionGoalLibraryEntity
    {
        public DeficientConditionGoalLibraryEntity()
        {
            DeficientConditionGoals = new HashSet<DeficientConditionGoalEntity>();
            DeficientConditionGoalLibrarySimulationJoins = new HashSet<DeficientConditionGoalLibrarySimulationEntity>();
        }

        public Guid Id { get; set; }

        public virtual ICollection<DeficientConditionGoalEntity> DeficientConditionGoals { get; set; }
        public virtual ICollection<DeficientConditionGoalLibrarySimulationEntity> DeficientConditionGoalLibrarySimulationJoins { get; set; }
    }
}
