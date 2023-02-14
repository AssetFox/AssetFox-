using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
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
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
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

        [Fact]
        public void GetSyncedScenarioDataset_RowToUpdate_ReturnsUpdatedRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            dto2.Name = "Updated name";
            var dto3 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            dto3.Name = "Updated name";
            var dtos = new List<CashFlowRuleDTO> { dto1 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                UpdateRows = new List<CashFlowRuleDTO> { dto2 },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto3, resultDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToDelete_DeletesRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var dto = CashFlowRuleDtos.Rule(ruleId);
            var dtos = new List<CashFlowRuleDTO> { dto };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { ruleId },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RepoReturnsRule_ReturnedInResult()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, criterionLibraryId, distributionRuleId);
            var dto2 = CashFlowRuleDtos.Rule(ruleId, criterionLibraryId, distributionRuleId);
            var dtos = new List<CashFlowRuleDTO> { dto1 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();

            var result = pagingService.GetSyncedScenarioDataSet(scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto2, resultDto);
        }

        [Fact]
        public void TooManyRulesForPage_Truncates()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto1Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2 = CashFlowRuleDtos.Rule();
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage= 1,
                Page = 1,
            };

            var result = pagingService.GetScenarioPage(scenarioId, pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto1Clone, item);
        }

        [Fact]
        public void RequestForSecondPage_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule();
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 2,
            };

            var result = pagingService.GetScenarioPage(scenarioId, pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto2Clone, item);
        }
    }
}
