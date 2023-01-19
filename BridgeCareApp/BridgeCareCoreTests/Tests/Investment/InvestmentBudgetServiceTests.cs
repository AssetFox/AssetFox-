using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Paging;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentBudgetServiceTests
    {
        private static InvestmentBudgetsService CreateInvestmentBudgetsService(Mock<IUnitOfWork> unitOfWork)
        {
            var hubService = HubServiceMocks.Default();
            var investmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var service = new InvestmentBudgetsService(unitOfWork.Object, expressionValidationService.Object, hubService, investmentDefaultDataService.Object);
            return service;
        }

        private static InvestmentPagingService CreateInvestmentPagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var investmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
            var pagingService = new InvestmentPagingService(unitOfWork.Object, investmentDefaultDataService.Object);
            return pagingService;
        }

        [Fact]
        public void GetSyncedInvestmentDataset_RepoReturnsABudget_BudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New();
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = new BudgetDTO
            {
                Id = budgetId,
                Name = budgetName,
                BudgetAmounts = new List<BudgetAmountDTO>(),
            };
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var service = CreateInvestmentBudgetsService(unitOfWork);
            var pagingService = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel();
            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);
            var returnedBudget = result.Single();
            Assert.Equal(budget, returnedBudget);
        }

        [Fact]
        public void GetSyncedInvestmentDataset_RepoReturnsABudgetButBudgetIsDeletedInRequest_BudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New();
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = new BudgetDTO
            {
                Id = budgetId,
                Name = budgetName,
            };
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                BudgetsForDeletion = new List<Guid> { budgetId },
            };
            var result = service.GetSyncedScenarioDataSet(simulationId, request);
            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedInvestmentDataset_BudgetAddedInRequest_BudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New();
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = new BudgetDTO
            {
                Id = budgetId,
                Name = budgetName,
                BudgetAmounts = new List<BudgetAmountDTO>(),
            };
            var emptyBudgets = new List<BudgetDTO>();
            var addedBudgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(emptyBudgets);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                AddedBudgets = addedBudgets,
            };
            var result = service.GetSyncedScenarioDataSet(simulationId, request);
            var resultBudget = result.Single();
            Assert.Equal(budget, resultBudget);
        }


        [Fact]
        public void GetSyncedInvestmentDataset_BudgetModifiedInRequest_ModifiedBudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New();
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = new BudgetDTO
            {
                Id = budgetId,
                Name = budgetName,
                BudgetAmounts = new List<BudgetAmountDTO>(),
            };
            var budgets = new List<BudgetDTO> { budget };
            var modifiedBudget = new BudgetDTO
            {
                Id = budgetId,
                Name = "Updated budget",
                BudgetAmounts = new List<BudgetAmountDTO>(),
            };
            var modifiedBudgets = new List<BudgetDTO> { modifiedBudget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                UpdatedBudgets = modifiedBudgets,
            };
            var result = service.GetSyncedScenarioDataSet(simulationId, request);
            var resultBudget = result.Single();
            ObjectAssertions.Equivalent(modifiedBudget, resultBudget);
        }
    }
}
