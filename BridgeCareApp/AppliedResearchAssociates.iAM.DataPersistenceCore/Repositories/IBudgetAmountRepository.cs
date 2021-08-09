using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetAmountRepository
    {
        void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId);

        void UpsertOrDeleteBudgetAmounts(Dictionary<Guid, List<BudgetAmountDTO>> budgetAmountsPerBudgetId, Guid libraryId);

        List<BudgetAmountEntity> GetBudgetAmountsByBudgetLibraryId(Guid libraryId);
    }
}
