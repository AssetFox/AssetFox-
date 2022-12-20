using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationRepositoryTests
    {
        [Fact]
        public void GetSimulationNameOrId_SimulationNotInDb_GetsId()
        {
            var simulationId = Guid.NewGuid();
            var nameOrId = TestHelper.UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            Assert.Contains(simulationId.ToString(), nameOrId);
        }
    }
}
