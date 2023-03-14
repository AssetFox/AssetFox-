using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    internal class BudgetAmountTestSetup
    {
        public static void SetupSingleAmountForBudget(UnitOfDataPersistenceWork unitOfWork, Guid simulationId, string budgetName, Guid budgetId, Guid budgetAmountId)
        {
            var budgetAmount = BudgetAmountDtos.ForBudgetNameAndYear(budgetName, 2023, 1234, budgetAmountId);
            var budgetAmountsPerBudgetId = new Dictionary<Guid, List<BudgetAmountDTO>>
            {
                {budgetId, new List<BudgetAmountDTO>{budgetAmount } }
            };
            unitOfWork.BudgetAmountRepo.UpsertOrDeleteScenarioBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
        }
    }
}
