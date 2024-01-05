using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationReportDetailRepositoryTests
    {
        [Fact]
        public void UpsertSimulationReportDetail_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var dto = SimulationReportDetailDtos.Dto(simulationId);

            TestHelper.UnitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

            var entityAfter = TestHelper.UnitOfWork.Context.SimulationReportDetail.Single(srd => srd.SimulationId == simulationId);
            Assert.Equal(dto.Status, entityAfter.Status);
        }
    }
}
