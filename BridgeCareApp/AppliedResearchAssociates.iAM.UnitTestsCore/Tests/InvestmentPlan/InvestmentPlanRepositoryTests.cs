using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
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

        [Fact]
        public void GetSimulationInvestmentPlan_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlanDto = TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulation.Id);
            investmentPlanDto.NumberOfYearsInAnalysisPeriod = 2;
            TestHelper.UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlanDto, simulation.Id);

            TestHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);

            var simulationInvestmentPlan = simulation.InvestmentPlan;
            Assert.Equal(2, simulationInvestmentPlan.NumberOfYearsInAnalysisPeriod);
        }
    }
}
