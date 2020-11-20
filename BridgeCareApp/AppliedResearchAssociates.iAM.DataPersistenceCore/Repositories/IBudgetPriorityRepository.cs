using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPriorityRepository
    {
        void CreateBudgetPriorityLibrary(string name, string simulationName);
        void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, string simulationName);
    }
}
