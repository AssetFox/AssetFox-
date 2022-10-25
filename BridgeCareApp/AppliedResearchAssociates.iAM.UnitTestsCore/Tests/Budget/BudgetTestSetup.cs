using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetTestSetup
    {
        public static void AddBudgetToLibrary(
            UnitOfDataPersistenceWork unitOfWork,
            Guid libraryId,
            Guid? budgetId = null)
        {
            var resolvedBudgetId = budgetId ?? Guid.NewGuid();
            var budgetName = RandomStrings.WithPrefix("Budget");
            var amount = new BudgetAmountDTO
            {
                BudgetName = budgetName,
                Id = Guid.NewGuid(),
                Year = 2022,
                Value = 123456.78m,
            };
            var amounts = new List<BudgetAmountDTO> { amount };
            var budget = new BudgetDTO
            {
                Id = resolvedBudgetId,
                Name = budgetName,
                BudgetAmounts = amounts,
            };
            var budgets = new List<BudgetDTO> { budget };
            unitOfWork.BudgetRepo.UpsertOrDeleteBudgets(budgets, libraryId);
        }
    }
}
