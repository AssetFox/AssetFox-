using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationRepositoryGetSharedScenariosTests
    {
        [Fact]
        public async Task GetSharedScenarios_NoAdminOrSimulationAccess_SimulationInDbOwnedByDifferentUser_DoesNotGet()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();

            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var sharedSimulations = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(false, false);
            Assert.Empty(sharedSimulations.Where(s => s.Id == simulationId));
        }


        [Fact]
        public async Task GetSharedScenarios_SimulationAccess_SimulationInDbOwnedByDifferentUser_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var sharedSimulations = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(false, true);

            var sharedSimulation = sharedSimulations.SingleOrDefault(s => s.Id == simulationId);
            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, sharedSimulation);
        }

        [Fact]
        public async Task GetSharedScenarios_AdminAccess_SimulationInDbOwnedByDifferentUser_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var sharedSimulations = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(false, true);

            var sharedSimulation = sharedSimulations.SingleOrDefault(s => s.Id == simulationId);
            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, sharedSimulation);
        }

        [Fact]
        public async Task GetSharedScenarios_UserIsOwner_DoesNotGet()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user.Id, networkId);

            var sharedSimulations = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(false, false);

            var sharedSimulation = sharedSimulations.SingleOrDefault(s => s.Id == simulationId);
            Assert.Null(sharedSimulation);
        }
    }
}
