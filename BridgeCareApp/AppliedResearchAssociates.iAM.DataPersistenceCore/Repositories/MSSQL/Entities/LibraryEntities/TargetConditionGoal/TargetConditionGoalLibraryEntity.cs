using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal
{
    public class TargetConditionGoalLibraryEntity : LibraryEntity
    {
        public TargetConditionGoalLibraryEntity()
        {
            TargetConditionGoals = new HashSet<TargetConditionGoalEntity>();
        }

        public virtual ICollection<TargetConditionGoalEntity> TargetConditionGoals { get; set; }
    }
}
