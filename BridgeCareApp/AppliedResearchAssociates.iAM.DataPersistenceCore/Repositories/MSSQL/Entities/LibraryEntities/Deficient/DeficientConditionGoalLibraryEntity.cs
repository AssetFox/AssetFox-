using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DeficientConditionGoalLibraryEntity : LibraryEntity
    {
        public DeficientConditionGoalLibraryEntity()
        {
            DeficientConditionGoals = new HashSet<DeficientConditionGoalEntity>();
            //DeficientConditionGoalLibrarySimulationJoins = new HashSet<DeficientConditionGoalLibrarySimulationEntity>();
        }

        public virtual ICollection<DeficientConditionGoalEntity> DeficientConditionGoals { get; set; }

        //public virtual ICollection<DeficientConditionGoalLibrarySimulationEntity> DeficientConditionGoalLibrarySimulationJoins { get; set; }
    }
}
