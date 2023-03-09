using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority
{
    public class BudgetPriorityLibraryEntity : LibraryEntity
    {
        public BudgetPriorityLibraryEntity()
        {
            BudgetPriorities = new HashSet<BudgetPriorityEntity>();
            Users = new HashSet<BudgetPriorityLibraryUserEntity>();
        }

        public virtual ICollection<BudgetPriorityEntity> BudgetPriorities { get; set; }
        public virtual ICollection<BudgetPriorityLibraryUserEntity> Users { get; set; }
    }
}
