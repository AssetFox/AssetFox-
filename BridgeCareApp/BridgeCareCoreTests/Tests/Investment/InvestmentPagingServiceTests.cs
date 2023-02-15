using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
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
    public class InvestmentPagingServiceTests
    {
        private static InvestmentPagingService CreateInvestmentPagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var investmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
            var pagingService = new InvestmentPagingService(unitOfWork.Object, investmentDefaultDataService.Object);
            return pagingService;
        }

        [Fact]
        public void GetSyncedScenarioDataSet_RepoReturnsABudget_BudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
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
            var pagingService = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel();

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedBudget = result.Single();
            Assert.Equal(budget, returnedBudget);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_RepoReturnsABudgetButBudgetIsDeletedInRequest_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
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
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                BudgetsForDeletion = new List<Guid> { budgetId },
            };
            var result = service.GetSyncedScenarioDataSet(simulationId, request);
            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_BudgetAddedInRequest_BudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
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
        public void GetSyncedScenarioDataSet_BudgetModifiedInRequest_ModifiedBudgetInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
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
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                UpdatedBudgets = modifiedBudgets,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, request);

            var resultBudget = result.Single();
            ObjectAssertions.Equivalent(modifiedBudget, resultBudget);
        }


        [Fact]
        public void GetSyncedScenarioDataSetButRequestHasLibraryId_GetsForLibrary()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
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
            var pagingService = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel();

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedBudget = result.Single();
            Assert.Equal(budget, returnedBudget);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_LibraryIdInRequest_GetsBudgetsForLibraryButFreshIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var amountId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 0, 2020, amountId);
            var budgetClone = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 2, amountId);
            var budgetLibrary = BudgetLibraryDtos.New();
            budgetLibrary.Budgets.Add(budget);
            budgetRepo.Setup(br => br.GetBudgetLibrary(libraryId)).Returns(budgetLibrary);
            var pagingService = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                LibraryId = libraryId,
                FirstYearAnalysisBudgetShift = 3,
            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedBudget = result.Single();
            ObjectAssertions.EquivalentExcluding(budgetClone, returnedBudget, x => x.Id, x => x.BudgetAmounts[0].Id);
            Assert.NotEqual(budgetClone.Id, returnedBudget.Id);
            Assert.NotEqual(budgetClone.BudgetAmounts[0].Id, returnedBudget.BudgetAmounts[0].Id);
        }
    }
}
