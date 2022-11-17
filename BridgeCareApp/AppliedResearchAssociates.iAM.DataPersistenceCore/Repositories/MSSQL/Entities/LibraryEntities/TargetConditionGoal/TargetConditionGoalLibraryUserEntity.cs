using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal
{
    public class TargetConditionGoalLibraryUserEntity : BaseEntity
    {
        public Guid TargetConditionGoalLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }

        public virtual TargetConditionGoalLibraryEntity TargetConditionGoalLibrary { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
