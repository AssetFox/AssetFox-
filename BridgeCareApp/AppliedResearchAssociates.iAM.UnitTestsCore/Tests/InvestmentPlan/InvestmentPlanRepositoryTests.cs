using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.InvestmentPlan
{
    public class InvestmentPlanRepositoryTests
    {
        [Fact]
        public void UpsertInvestmentPlan_SimulationInDb_Inserts()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var investmentPlan = new InvestmentPlanDTO
            {
                Id = Guid.NewGuid(),
            };

            TestHelper.UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlan, simulation.Id);

            var investmentPlanAfter = TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulation.Id);
            ObjectAssertions.Equivalent(investmentPlan, investmentPlanAfter);
        }
    }
}
