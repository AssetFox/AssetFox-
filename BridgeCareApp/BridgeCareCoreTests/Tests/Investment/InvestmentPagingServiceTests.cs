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
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Paging;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentPagingServiceTests
    {
        private static InvestmentPagingService CreateInvestmentPagingService(Mock<IUnitOfWork> unitOfWork, Mock<IInvestmentDefaultDataService> investmentDefaultDataService = null)
        {
            investmentDefaultDataService ??= new Mock<IInvestmentDefaultDataService>();
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
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2020, 54321, amountId);
            var budgetClone = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 54321, amountId);
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


        [Fact]
        public void GetScenarioPage_SortByYear_AmountsSortedByYear()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var investmentDefaultData = new InvestmentDefaultData
            {
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
            };
            var investmentDefaultDataService = InvestmentDefaultDataServiceMocks.New(investmentDefaultData);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = BudgetDtos.New(budgetId);
            var amount2023 = BudgetAmountDtos.ForBudgetAndYear(budget, 2023);
            var amount2024 = BudgetAmountDtos.ForBudgetAndYear(budget, 2024);
            var amount2025 = BudgetAmountDtos.ForBudgetAndYear(budget, 2025);
            var amounts = new List<BudgetAmountDTO> { amount2023, amount2025, amount2024 };
            budget.BudgetAmounts.AddRange(amounts);
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var pagingService = CreateInvestmentPagingService(unitOfWork, investmentDefaultDataService);
            var request = new InvestmentPagingSyncModel
            {
                Investment = new InvestmentPlanDTO
                {
                    FirstYearOfAnalysisPeriod = 2023,
                    NumberOfYearsInAnalysisPeriod = 1,
                }
            };
            var pageRequest = new InvestmentPagingRequestModel
            {
                sortColumn = "year",
                SyncModel = request,
            };

            var result = pagingService.GetScenarioPage(simulationId, pageRequest);

            var expectedBudget = BudgetDtos.New(budgetId);
            var expectedAmounts = new List<BudgetAmountDTO> { amount2023, amount2024, amount2025 };
            expectedBudget.BudgetAmounts.AddRange(expectedAmounts);
            var expectedInvestmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = 2023,
                NumberOfYearsInAnalysisPeriod = 1,
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
                ShouldAccumulateUnusedBudgetAmounts = false,
            };
            var expectedBudgets = new List<BudgetDTO> { expectedBudget };
            var expected = new InvestmentPagingPageModel
            {
                FirstYear = 2023,
                LastYear = 2025,
                InvestmentPlan = expectedInvestmentPlan,
                TotalItems = 3,
                Items = expectedBudgets,
            };
            ObjectAssertions.EquivalentExcluding(expected, result, x=> x.InvestmentPlan.Id);
        }

        [Fact]
        public void GetScenarioPage_SortByYearDescending_AmountsSortedByYearDescending()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var investmentDefaultData = new InvestmentDefaultData
            {
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
            };
            var investmentDefaultDataService = InvestmentDefaultDataServiceMocks.New(investmentDefaultData);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.Length11();
            var budget = BudgetDtos.New(budgetId);
            var amount2023 = BudgetAmountDtos.ForBudgetAndYear(budget, 2023);
            var amount2024 = BudgetAmountDtos.ForBudgetAndYear(budget, 2024);
            var amount2025 = BudgetAmountDtos.ForBudgetAndYear(budget, 2025);
            var amounts = new List<BudgetAmountDTO> { amount2023, amount2025, amount2024 };
            budget.BudgetAmounts.AddRange(amounts);
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var pagingService = CreateInvestmentPagingService(unitOfWork, investmentDefaultDataService);
            var request = new InvestmentPagingSyncModel
            {
                Investment = new InvestmentPlanDTO
                {
                    FirstYearOfAnalysisPeriod = 2023,
                    NumberOfYearsInAnalysisPeriod = 5,
                }
            };
            var pageRequest = new InvestmentPagingRequestModel
            {
                sortColumn = "year",
                isDescending = true,
                SyncModel = request,
            };

            var result = pagingService.GetScenarioPage(simulationId, pageRequest);

            var expectedBudget = BudgetDtos.New(budgetId);
            var expectedAmounts = new List<BudgetAmountDTO> { amount2025, amount2024, amount2023 };
            expectedBudget.BudgetAmounts.AddRange(expectedAmounts);
            var expectedInvestmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = 2023,
                NumberOfYearsInAnalysisPeriod = 5,
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
                ShouldAccumulateUnusedBudgetAmounts = false,
            };
            var expectedBudgets = new List<BudgetDTO> { expectedBudget };
            var expected = new InvestmentPagingPageModel
            {
                FirstYear = 2023,
                LastYear = 2025,
                InvestmentPlan = expectedInvestmentPlan,
                TotalItems = 3,
                Items = expectedBudgets,
            };
            ObjectAssertions.EquivalentExcluding(expected, result, x => x.InvestmentPlan.Id);
        }

    }
}
