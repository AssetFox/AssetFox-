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
    public class SimulationRepositoryGetCurrentUserOrSharedScenarioTests
    {
        [Fact]
        public async Task GetCurrentUserOrSharedScenario_NoAdminOrSimulationAccess_SimulationInDbOwnedByDifferentUser_DoesNotGet()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();

            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, false, false);

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetCurrentUserOrSharedScenario_SimulationAccess_SimulationInDbOwnedByDifferentUser_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, false, true);

            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, actual);
        }

        [Fact]
        public async Task GetCurrentUserOrSharedScenario_AdminAccess_SimulationInDbOwnedByDifferentUser_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, true, false);

            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, actual);
        }

        [Fact]
        public async Task GetCurrentUserOrSharedScenario_UserIsOwner_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user.Id, networkId);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, false, false);

            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, actual);
        }

        [Fact]
        public async Task GetCurrentUserOrSharedScenario_UserIsUserOfSimulation_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = NetworkTestSetup.NetworkId;
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user1.Id, networkId);
            var simulationUser = SimulationUserDtos.Dto(user2.Id, user2.Username);
            simulation.Users.Add(simulationUser);
            TestHelper.UnitOfWork.SimulationRepo.UpdateSimulationAndPossiblyUsers(simulation);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, false, false);

            var simulationAnotherWay = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            ObjectAssertions.Equivalent(simulationAnotherWay, actual);
        }
    }
}
