using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority
{
    public static class BudgetPriorityTestSetup
    {
        public static BudgetPriorityDTO SetupSingleBudgetPriorityForSimulationInDb(Guid simulationId)
        {
            var budgetPriority = BudgetPriorityDtos.New();
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId);
            return budgetPriority;
        }
    }
}
