using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetAmountDtos
    {
        public static BudgetAmountDTO ForBudgetAndYear(BudgetDTO budget, int year, decimal value = 1234)
        {
            var amount = new BudgetAmountDTO
            {
                Id = Guid.NewGuid(),
                BudgetName = budget.Name,
                Year = year,
                Value = value
            };
            return amount;
        }
    }
}
