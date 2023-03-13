using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using BridgeCareCore.Models;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.WorkQueue;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class SimulationAnalysisServiceIntegrationTests
    {
        private SimulationAnalysisService CreateService()
        {
            var queue = new SequentialWorkQueue();
            var service = new SimulationAnalysisService(
                TestHelper.UnitOfWork,
                queue);
            return service;
        }

        [Fact]
        public async Task CreateAndRunPermitted_SimulationExists_Ok()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var userId = user.Id;
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, userId);
            var service = CreateService();

            var userInfo = new UserInfo
            {
                Name = user.Username,
                HasAdminAccess = true,
                HasSimulationAccess = true,
                Email = "Foo@bar.Com",
            };
            var expectedWorkItem = new AnalysisWorkItem(
                NetworkTestSetup.NetworkId, simulationId, userInfo);

            var result = service.CreateAndRunPermitted(NetworkTestSetup.NetworkId, simulationId, userInfo);

            var resultUserInfo = result.UserInfo;
            ObjectAssertions.Equivalent(userInfo, resultUserInfo);
        }

        [Fact]
        public async Task CreateAndRunPermitted_SimulationDoesNotExist_Throws()
        {
            var service = CreateService();
            var simulationId = Guid.NewGuid();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);

            var userInfo = new UserInfo
            {
                Name = user.Username,
                HasAdminAccess = true,
                HasSimulationAccess = true,
                Email = "Foo@bar.Com",
            };
            TestHelper.UnitOfWork.SetUser(user.Username);
            var exception = Assert.Throws<RowNotInTableException>(() =>
               service.CreateAndRunPermitted(NetworkTestSetup.NetworkId,
               simulationId, userInfo));
            Assert.Equal(SimulationRepository.NoSimulationWasFoundForTheGivenScenario, exception.Message);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]   
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task CreateAndRunPermitted_SimulationExistsButNoUser_Throws(bool isAdmin, bool hasSimulationAccess)
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var service = CreateService();
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user.Id);
            var user2Info = new UserInfo
            {
                Name = user2.Username,
                HasAdminAccess = isAdmin,
                HasSimulationAccess = hasSimulationAccess,
                Email = "Foo@bar.Com",
            };
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var exception = Assert.Throws<UnauthorizedAccessException>(() =>
               service.CreateAndRunPermitted(NetworkTestSetup.NetworkId,
               simulationId, user2Info));
            Assert.Equal(SimulationAnalysisService.YouAreNotAuthorizedToModifyThisSimulation, exception.Message);
        }


        [Fact]
        public async Task CreateAndRunPermitted_SimulationExistsButUserCantModify_Throws()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var service = CreateService();
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user.Id);
            var user2Info = new UserInfo
            {
                Name = user2.Username,
                HasAdminAccess = false,
                HasSimulationAccess = false,
                Email = "Foo@bar.Com",
            };
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationUser2 = new SimulationUserEntity
            {
                CanModify = false,
                IsOwner = false,
                SimulationId = simulationId,
                UserId = user2.Id
            };
            TestHelper.UnitOfWork.Context.Add(simulationUser2);
            TestHelper.UnitOfWork.Context.SaveChanges();
            var exception = Assert.Throws<UnauthorizedAccessException>(() =>
               service.CreateAndRunPermitted(NetworkTestSetup.NetworkId,
               simulationId, user2Info));
            Assert.Equal(SimulationAnalysisService.YouAreNotAuthorizedToModifyThisSimulation, exception.Message);
        }

        [Fact]
        public async Task CreateAndRunPermitted_SimulationExistsAndUserCanModify_Succeeds()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var service = CreateService();
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName, user.Id);
            var user2Info = new UserInfo
            {
                Name = user2.Username,
                HasAdminAccess = false,
                HasSimulationAccess = false,
                Email = "Foo@bar.Com",
            };
            TestHelper.UnitOfWork.SetUser(user2.Username);
            var simulationUser2 = new SimulationUserEntity
            {
                CanModify = true,
                IsOwner = false,
                SimulationId = simulationId,
                UserId = user2.Id
            };
            TestHelper.UnitOfWork.Context.Add(simulationUser2);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var result = service.CreateAndRunPermitted(NetworkTestSetup.NetworkId, simulationId, user2Info);

            var resultUserInfo = result.UserInfo;
            ObjectAssertions.Equivalent(user2Info, resultUserInfo);
        }
    }
}
