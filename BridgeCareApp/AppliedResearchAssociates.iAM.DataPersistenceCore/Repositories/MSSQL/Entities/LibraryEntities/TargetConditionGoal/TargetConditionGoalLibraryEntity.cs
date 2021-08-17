using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TargetConditionGoalLibraryEntity : LibraryEntity
    {
        public TargetConditionGoalLibraryEntity()
        {
            TargetConditionGoals = new HashSet<TargetConditionGoalEntity>();
            //TargetConditionGoalLibrarySimulationJoins = new HashSet<TargetConditionGoalLibrarySimulationEntity>();
        }

        public virtual ICollection<TargetConditionGoalEntity> TargetConditionGoals { get; set; }

        //public virtual ICollection<TargetConditionGoalLibrarySimulationEntity> TargetConditionGoalLibrarySimulationJoins { get; set; }
    }
}
