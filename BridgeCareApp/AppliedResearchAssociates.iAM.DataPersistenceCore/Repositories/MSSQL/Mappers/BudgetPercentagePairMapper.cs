using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetPercentagePairMapper
    {
        public static BudgetPercentagePairEntity ToEntity(this BudgetPercentagePair domain, Guid budgetPriorityId, Guid budgetId) =>
            new BudgetPercentagePairEntity
            {
                Id = domain.Id,
                ScenarioBudgetPriorityId = budgetPriorityId,
                ScenarioBudgetId = budgetId,
                Percentage = domain.Percentage
            };

        public static BudgetPercentagePairEntity ToEntity(this BudgetPercentagePairDTO dto, Guid budgetPriorityId, BaseEntityProperties baseEntityProperties = null) =>
            new BudgetPercentagePairEntity
            {
                Id = dto.Id,
                ScenarioBudgetPriorityId = budgetPriorityId,
                ScenarioBudgetId = dto.BudgetId,
                Percentage = dto.Percentage
            };

        public static BudgetPercentagePairDTO ToDto(this BudgetPercentagePairEntity entity) =>
            new BudgetPercentagePairDTO
            {
                Id = entity.Id,
                Percentage = entity.Percentage,
                BudgetId = entity.ScenarioBudgetId,
                BudgetName = entity.ScenarioBudget != null
                    ? entity.ScenarioBudget.Name
                    : ""
            };

        public static void FillBudgetPercentagePair(this BudgetPercentagePairEntity entity, InvestmentPlan investmentPlan,
            BudgetPriority budgetPriority)
        {
            var budget = investmentPlan.Budgets.Single(_ => _.Id == entity.ScenarioBudgetId);
            var budgetPercentagePair = budgetPriority.GetBudgetPercentagePair(budget);
            budgetPercentagePair.Id = entity.Id;
            budgetPercentagePair.Percentage = entity.Percentage;
        }
    }
}
