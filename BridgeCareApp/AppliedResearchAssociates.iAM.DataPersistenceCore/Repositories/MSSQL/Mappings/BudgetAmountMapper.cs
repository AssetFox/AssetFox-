using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetAmountMapper
    {
        public static BudgetAmountEntity ToEntity(this BudgetAmount domain, Guid budgetId, int year) =>
            new BudgetAmountEntity { Id = domain.Id, BudgetId = budgetId, Value = domain.Value, Year = year };

        public static BudgetAmountEntity ToEntity(this BudgetAmountDTO dto, Guid budgetId) =>
            new BudgetAmountEntity { Id = dto.Id, BudgetId = budgetId, Value = dto.Value, Year = dto.Year };

        public static BudgetAmountDTO ToDto(this BudgetAmountEntity entity, string budgetName) =>
            new BudgetAmountDTO { Id = entity.Id, BudgetName = budgetName, Value = entity.Value, Year = entity.Year };
    }
}
