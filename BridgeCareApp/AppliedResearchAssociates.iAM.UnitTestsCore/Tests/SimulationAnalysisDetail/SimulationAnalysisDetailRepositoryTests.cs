using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationAnalysisDetail
{
    public class SimulationAnalysisDetailRepositoryTests
    {
        [Fact]
        public void UpsertSimulationAnalysisDetail_ThenGet_Same()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var dto = SimulationAnalysisDetailDtos.ForSimulation(simulationId);

            TestHelper.UnitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(dto);

            var dtoAfter = TestHelper.UnitOfWork.SimulationAnalysisDetailRepo.GetSimulationAnalysisDetail(simulationId);
            ObjectAssertions.Equivalent(dtoAfter, dto);
        }
    }
}
