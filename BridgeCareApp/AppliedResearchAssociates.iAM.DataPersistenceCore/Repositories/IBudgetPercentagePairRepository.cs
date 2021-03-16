﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPercentagePairRepository
    {
        void CreateBudgetPercentagePairs(Dictionary<Guid, List<(Guid budgetId, BudgetPercentagePair percentagePair)>> percentagePairPerBudgetIdPerPriorityId);

        void UpsertOrDeleteBudgetPercentagePairs(Dictionary<Guid, List<BudgetPercentagePairDTO>> percentagePairsPerPriorityId, Guid libraryId, Guid? userId = null);
    }
}
