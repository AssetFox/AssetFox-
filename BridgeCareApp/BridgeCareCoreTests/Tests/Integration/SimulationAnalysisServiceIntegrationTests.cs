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
            Assert.Equal(SimulationAnalysisService.NoSimulationFoundForGivenScenario, exception.Message);
        }
    }
}
