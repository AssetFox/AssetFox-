using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests.CashFlowRule
{
    public class CashFlowPagingServiceTests
    {
        private CashFlowPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CashFlowPagingService(unitOfWork.Object);
            return service;
        }
        [Fact]
        public void GetSyncedScenarioDataset_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }
    }
}
