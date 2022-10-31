using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioBudgetTestSetup
    {
        public static void CreateScenarioBudgets(IUnitOfWork unitOfWork, List<Budget> budgets, Guid simulationId)
        {
            unitOfWork.BudgetRepo.CreateScenarioBudgets(budgets, simulationId);
        }
    }
}
