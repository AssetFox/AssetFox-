using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPercentagePairRepository
    {
        void CreateBudgetPercentagePairs(Dictionary<Guid, List<(Guid budgetId, BudgetPercentagePair percentagePair)>> percentagePairPerBudgetIdPerPriorityId);

        void UpsertOrDeleteBudgetPercentagePairs(Dictionary<Guid, List<BudgetPercentagePairDTO>> percentagePairsPerPriorityId, Guid libraryId);
    }
}
