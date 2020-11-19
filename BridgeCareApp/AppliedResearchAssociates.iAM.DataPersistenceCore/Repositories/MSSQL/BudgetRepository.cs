using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetRepository : MSSQLRepository, IBudgetRepository
    {
        private readonly IBudgetAmountRepository _budgetAmountRepo;

        public BudgetRepository(IBudgetAmountRepository budgetAmountRepo, IAMContext context) : base(context) =>
            _budgetAmountRepo = budgetAmountRepo ?? throw new ArgumentNullException(nameof(budgetAmountRepo));

        public void CreateBudgets(List<Budget> budgets, Guid investmentPlanId)
        {
            if (!Context.InvestmentPlan.Any(_ => _.Id == investmentPlanId))
            {
                throw new RowNotInTableException($"No investment plan found having id {investmentPlanId}.");
            }

            var budgetAmountsPerBudgetId = new Dictionary<Guid, List<BudgetAmount>>();

            var budgetEntities = budgets.Select(_ =>
            {
                var budgetEntity = _.ToEntity(investmentPlanId);

                if (_.YearlyAmounts.Any())
                {
                    budgetAmountsPerBudgetId.Add(budgetEntity.Id, _.YearlyAmounts.ToList());
                }

                return budgetEntity;
            }).ToList();

            Context.Budget.AddRange(budgetEntities);
            Context.SaveChanges();

            if (budgetAmountsPerBudgetId.Values.Any())
            {
                _budgetAmountRepo.CreateBudgetAmounts(budgetAmountsPerBudgetId, investmentPlanId);
            }
        }
    }
}
