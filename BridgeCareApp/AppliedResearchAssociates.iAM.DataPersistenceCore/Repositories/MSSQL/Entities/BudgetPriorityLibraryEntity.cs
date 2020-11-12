using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPriorityLibraryEntity
    {
        public BudgetPriorityLibraryEntity()
        {
            BudgetPriorities = new HashSet<BudgetPriorityEntity>();
            BudgetPriorityLibrarySimulationJoins = new HashSet<BudgetPriorityLibrarySimulationEntity>();
        }

        public Guid Id { get; set; }

        public virtual ICollection<BudgetPriorityEntity> BudgetPriorities { get; set; }
        public virtual ICollection<BudgetPriorityLibrarySimulationEntity> BudgetPriorityLibrarySimulationJoins { get; set; }
    }
}
