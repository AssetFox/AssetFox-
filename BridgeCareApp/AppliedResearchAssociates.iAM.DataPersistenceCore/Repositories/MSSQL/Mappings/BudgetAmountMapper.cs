using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetAmountMapper
    {
        public static BudgetAmountEntity ToEntity(this BudgetAmount domain, Guid budgetId, int year) =>
            new BudgetAmountEntity
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetId,
                Value = domain.Value,
                Year = year
            };

        public static BudgetAmount ToDomain(this BudgetAmountEntity entity) =>
            new BudgetAmount
            {
                Value = entity.Value
            };
    }
}
