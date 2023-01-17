using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient
{
    public class DeficientConditionGoalLibraryUserEntity : BaseEntity
    {
        public Guid DeficientConditionGoalLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }
        public virtual DeficientConditionGoalLibraryEntity DeficientConditionGoalLibrary { get; set; }
        public virtual UserEntity User { get; set; }

    }
}
