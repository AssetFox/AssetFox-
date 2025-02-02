﻿using System;
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
        public void GetSyncedScenarioDataSet_BudgetAmountModifiedInRequest_ModifiedBudgetAmountInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var amountId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2023, 123456, amountId);
            var budgets = new List<BudgetDTO> { budget };
            var updatedAmount = BudgetAmountDtos.ForBudgetAndYear(budget, 2023, 654321, amountId);
            var updatedAmounts = new List<BudgetAmountDTO> { updatedAmount };
            var updatedAmountDictionary = new Dictionary<string, List<BudgetAmountDTO>> { { "Budget", updatedAmounts } };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                UpdatedBudgetAmounts = updatedAmountDictionary,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, request);

            var resultBudget = result.Single();
            var resultAmount = resultBudget.BudgetAmounts.Single();
            Assert.Equal(654321, resultAmount.Value);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_BudgetAmountAddedInRequest_AddedBudgetAmountInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var amountId = Guid.NewGuid();
            var amountId2 = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2023, 123456, amountId);
            var amount2023 = budget.BudgetAmounts.Single();
            var budgets = new List<BudgetDTO> { budget };
            var addedAmount = BudgetAmountDtos.ForBudgetAndYear(budget, 2024, 654321, amountId2);
            var addedAmounts = new List<BudgetAmountDTO> { addedAmount };
            var addedAmountDictionary = new Dictionary<string, List<BudgetAmountDTO>> { { "Budget", addedAmounts } };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                AddedBudgetAmounts = addedAmountDictionary,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, request);

            var resultBudget = result.Single();
            var resultAmounts = resultBudget.BudgetAmounts;
            Assert.Equal(2, resultAmounts.Count);
            var resultAmount2023 = resultAmounts.Single(x => x.Id == amountId);
            var resultAmount2024 = resultAmounts.Single(x => x.Id == amountId2);
            ObjectAssertions.Equivalent(amount2023, resultAmount2023);
            ObjectAssertions.Equivalent(addedAmount, resultAmount2024);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_YearDeletedInRequest_BudgetAmountForYearNotInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var amountId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2023, 123456, amountId);
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var service = CreateInvestmentPagingService(unitOfWork);
            var request = new InvestmentPagingSyncModel
            {
                Deletionyears = new List<int> { 2023 },
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, request);

            var resultBudget = result.Single();
            Assert.Empty(resultBudget.BudgetAmounts);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_BudgetDeletedInRequest_BudgetNotInDataset()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var amountId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2023, 123456, amountId);
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
            ObjectAssertions.EquivalentExcluding(expected, result, x => x.InvestmentPlan.Id);
        }

        [Fact]
        public void GetScenarioPage_InvestmentPlanYearIsZero_ReturnsCurrentYear()
        {
            var year = DateTime.Now.Year;
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
            var thisYearAmount = BudgetAmountDtos.ForBudgetAndYear(budget, year);
            var amounts = new List<BudgetAmountDTO> { thisYearAmount, };
            budget.BudgetAmounts.AddRange(amounts);
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var pagingService = CreateInvestmentPagingService(unitOfWork, investmentDefaultDataService);
            var request = new InvestmentPagingSyncModel
            {
                Investment = new InvestmentPlanDTO
                {
                    FirstYearOfAnalysisPeriod = 0,
                    NumberOfYearsInAnalysisPeriod = 1,
                }
            };
            var pageRequest = new InvestmentPagingRequestModel
            {
                SyncModel = request,
            };

            var result = pagingService.GetScenarioPage(simulationId, pageRequest);

            if (DateTime.Now.Year!=year)
            {
                return;
            }
            var expectedBudget = BudgetDtos.New(budgetId);
            var expectedAmounts = new List<BudgetAmountDTO> { thisYearAmount };
            expectedBudget.BudgetAmounts.AddRange(expectedAmounts);
            var expectedInvestmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = year,
                NumberOfYearsInAnalysisPeriod = 1,
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
                ShouldAccumulateUnusedBudgetAmounts = false,
            };
            var expectedBudgets = new List<BudgetDTO> { expectedBudget };
            var expected = new InvestmentPagingPageModel
            {
                FirstYear = year,
                LastYear = year,
                InvestmentPlan = expectedInvestmentPlan,
                TotalItems = 1,
                Items = expectedBudgets,
            };
            ObjectAssertions.EquivalentExcluding(expected, result, x => x.InvestmentPlan.Id);
        }
        [Fact]
        public void GetScenarioPage_ZeroRowsPerPage_ReturnsAllBudgets()
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
            var thisYearAmount = BudgetAmountDtos.ForBudgetAndYear(budget, 2023);
            var amounts = new List<BudgetAmountDTO> { thisYearAmount, };
            budget.BudgetAmounts.AddRange(amounts);
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            var pagingService = CreateInvestmentPagingService(unitOfWork, investmentDefaultDataService);
            var request = new InvestmentPagingSyncModel
            {
                Investment = new InvestmentPlanDTO
                {
                    FirstYearOfAnalysisPeriod = 0,
                    NumberOfYearsInAnalysisPeriod = 1,
                }
            };
            var pageRequest = new InvestmentPagingRequestModel
            {
                RowsPerPage = 0,
                SyncModel = request,
            };
            var yearBefore = DateTime.Now.Year;

            var result = pagingService.GetScenarioPage(simulationId, pageRequest);

            var yearAfter = DateTime.Now.Year;
            var expectedBudget = BudgetDtos.New(budgetId);
            var expectedAmounts = new List<BudgetAmountDTO> { thisYearAmount };
            expectedBudget.BudgetAmounts.AddRange(expectedAmounts);
            var expectedInvestmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = DateTime.Now.Year,
                NumberOfYearsInAnalysisPeriod = 1,
                InflationRatePercentage = 3,
                MinimumProjectCostLimit = 100000,
                ShouldAccumulateUnusedBudgetAmounts = false,
            };
            var expectedBudgets = new List<BudgetDTO> { expectedBudget };
            var expected = new InvestmentPagingPageModel
            {
                FirstYear = 2023,
                LastYear = 2023,
                InvestmentPlan = expectedInvestmentPlan,
                TotalItems = 0,
                Items = expectedBudgets,
            };
            ObjectAssertions.EquivalentExcluding(expected, result, x => x.InvestmentPlan.Id, x => x.InvestmentPlan.FirstYearOfAnalysisPeriod);
            DoubleAssertions.Between(yearBefore, yearAfter, result.InvestmentPlan.FirstYearOfAnalysisPeriod);
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

        [Fact]
        public void GetScenarioPage_SortIsBudgetName_AmountsSortedByAmount()
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
            var budget = BudgetDtos.New(budgetId, "Budget");
            var amount2023 = BudgetAmountDtos.ForBudgetAndYear(budget, 2023, 1000000);
            var amount2024 = BudgetAmountDtos.ForBudgetAndYear(budget, 2024, 100000);
            var amount2025 = BudgetAmountDtos.ForBudgetAndYear(budget, 2025, 200000);
            var amounts = new List<BudgetAmountDTO> { amount2023, amount2024, amount2025 };
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
                sortColumn = "Budget",
                SyncModel = request,
            };

            var result = pagingService.GetScenarioPage(simulationId, pageRequest);

            var expectedBudget = BudgetDtos.New(budgetId);
            var expectedAmounts = new List<BudgetAmountDTO> { amount2023, amount2025, amount2024 };
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
            ObjectAssertions.EquivalentExcluding(expected, result, x => x.InvestmentPlan.Id);
        }

        [Fact]
        public void GetScenarioPage_SortIsNotBudgetName_Throws()
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
            var budget = BudgetDtos.New(budgetId, "Budget");
            var amount2023 = BudgetAmountDtos.ForBudgetAndYear(budget, 2023, 1000000);
            var amount2024 = BudgetAmountDtos.ForBudgetAndYear(budget, 2024, 100000);
            var amount2025 = BudgetAmountDtos.ForBudgetAndYear(budget, 2025, 200000);
            var amounts = new List<BudgetAmountDTO> { amount2023, amount2024, amount2025 };
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
                sortColumn = "Nonexistent budget",
                SyncModel = request,
            };

            Assert.Throws<NullReferenceException>(() => pagingService.GetScenarioPage(simulationId, pageRequest));
        }
    }
}
