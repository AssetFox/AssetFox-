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
        private GeneralWorkQueueService CreateService()
        {
            var queue = new SequentialWorkQueue<WorkQueueMetadata>();
            var fastQueue = new FastSequentialworkQueue<WorkQueueMetadata>();
            var service = new GeneralWorkQueueService(
                queue, fastQueue);
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
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, userId);
            var service = CreateService();

            var userInfo = new UserInfo
            {
                Name = user.Username,
                HasAdminAccess = true,
                HasSimulationAccess = true,
                Email = "Foo@bar.Com",
            };
            var expectedWorkItem = new AnalysisWorkItem(
                NetworkTestSetup.NetworkId, simulationId, userInfo, simulationName);

            var result = service.CreateAndRun(expectedWorkItem);
            var resultUser = result.UserId;
            Assert.Equal(user.Username, resultUser);
        }        
    }
}
