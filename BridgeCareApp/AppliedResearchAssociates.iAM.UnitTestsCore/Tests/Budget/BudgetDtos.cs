using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetDtos
    {
        public static BudgetDTO New(Guid? id = null, string name = "Budget")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new BudgetDTO
            {
                Id = resolveId,
                BudgetAmounts = new List<BudgetAmountDTO>(),
                Name = name,
            };
            return dto;
        }

        public static BudgetDTO WithSingleAmount(Guid id, string name, int year, decimal value, Guid? amountId = null)
        {
            var budget = New(id, name);
            var budgetAmount = BudgetAmountDtos.ForBudgetAndYear(budget, year, value, amountId);
            budget.BudgetAmounts.Add(budgetAmount);
            return budget;
        }
    }
}
