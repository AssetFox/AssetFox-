using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioBudgetTestSetup
    {
        public static void CreateScenarioBudgets(IUnitOfWork unitOfWork, List<Budget> budgets, Guid simulationId)
        {
            unitOfWork.BudgetRepo.CreateScenarioBudgets(budgets, simulationId);
        }

        public static void UpsertOrDeleteScenarioBudgets(
            IUnitOfWork unitOfWork,
            List<BudgetDTO> budgets,
            Guid simulationId
            )
        {
            unitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(
                budgets,
                simulationId);
        }
    }
}
