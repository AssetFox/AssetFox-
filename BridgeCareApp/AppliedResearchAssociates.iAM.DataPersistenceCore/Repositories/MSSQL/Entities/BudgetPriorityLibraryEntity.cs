using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPriorityLibraryEntity : LibraryEntity
    {
        public BudgetPriorityLibraryEntity()
        {
            BudgetPriorities = new HashSet<BudgetPriorityEntity>();
            BudgetPriorityLibrarySimulationJoins = new HashSet<BudgetPriorityLibrarySimulationEntity>();
        }

        public virtual ICollection<BudgetPriorityEntity> BudgetPriorities { get; set; }

        public virtual ICollection<BudgetPriorityLibrarySimulationEntity> BudgetPriorityLibrarySimulationJoins { get; set; }
    }
}
