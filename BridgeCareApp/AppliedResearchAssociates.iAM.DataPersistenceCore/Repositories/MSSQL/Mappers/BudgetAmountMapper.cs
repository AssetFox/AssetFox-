using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetAmountMapper
    {
        public static ScenarioBudgetAmountEntity ToScenarioEntity(this BudgetAmount domain, Guid budgetId, int year) =>
            new ScenarioBudgetAmountEntity { Id = domain.Id, ScenarioBudgetId = budgetId, Value = domain.Value, Year = year };

        public static BudgetAmountEntity ToLibraryEntity(this BudgetAmountDTO dto, Guid budgetId) =>
            new BudgetAmountEntity { Id = dto.Id, BudgetId = budgetId, Value = dto.Value, Year = dto.Year };

        public static ScenarioBudgetAmountEntity ToScenarioEntity(this BudgetAmountDTO dto, Guid budgetId, BaseEntityProperties baseEntityProperties = null)
        {
            var entity = new ScenarioBudgetAmountEntity
            {
                Id = dto.Id,
                ScenarioBudgetId = budgetId,
                Value = dto.Value,
                Year = dto.Year
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }

        public static BudgetAmountDTO ToDto(this BudgetAmountEntity entity, string budgetName) =>
            new BudgetAmountDTO { Id = entity.Id, BudgetName = budgetName, Value = entity.Value, Year = entity.Year };

        public static BudgetAmountDTO ToDto(this ScenarioBudgetAmountEntity entity, string budgetName) =>
            new BudgetAmountDTO { Id = entity.Id, BudgetName = budgetName, Value = entity.Value, Year = entity.Year };
    }
}
