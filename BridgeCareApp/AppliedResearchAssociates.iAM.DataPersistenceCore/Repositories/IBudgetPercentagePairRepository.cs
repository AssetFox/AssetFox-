using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPercentagePairRepository
    {
        void CreateBudgetPercentagePairs(
            List<((Guid priorityId, Guid budgetId) priorityIdBudgetIdTuple, BudgetPercentagePair budgetPercentagePair
                )> budgetPercentagePairPriorityIdBudgetIdTupleTuple);
    }
}
