using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPriorityRepository
    {
        void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid analysisMethodId);
    }
}
