using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationCloningTests
    {
        [Fact]
        public void SimulationInDb_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationEntity = TestHelper.UnitOfWork.Context.Simulation.Single(s => s.Name == newSimulationName);
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationEntity.Id);

            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
        }
    }
}
