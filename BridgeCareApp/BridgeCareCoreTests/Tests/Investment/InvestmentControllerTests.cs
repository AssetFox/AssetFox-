using System.Data;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCore.Services.Paging;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.General_Work_Queue;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using MoreLinq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentControllerTests
    {
        private InvestmentBudgetsService CreateService(Mock<IUnitOfWork> mockUnitOfWork)
        {
            var logger = new LogNLog();
            var hubService = HubServiceMocks.DefaultMock();
            var expressionValidationService = new ExpressionValidationService(mockUnitOfWork.Object, logger);
            var service = new InvestmentBudgetsService(
                mockUnitOfWork.Object,
                expressionValidationService,
                hubService.Object,
                new InvestmentDefaultDataService()
                );
            return service;
        }

        private InvestmentController CreateController(
            Mock<IUnitOfWork> mockUnitOfWork, Mock<IHttpContextAccessor> accessor = null)
        {
            var service = CreateService(mockUnitOfWork);
            var resolveAccessor = accessor ?? HttpContextAccessorMocks.DefaultMock();
            var hubService = HubServiceMocks.DefaultMock();
            var dataService = new InvestmentDefaultDataService();
            var security = EsecSecurityMocks.Admin;
            var pagingService = new InvestmentPagingService(mockUnitOfWork.Object, dataService);
            var claimHelper = ClaimHelperMocks.New();
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            var controller = new InvestmentController(
                service,
                pagingService,
                security,
                mockUnitOfWork.Object,
                hubService.Object,
                resolveAccessor.Object,
                dataService,
                claimHelper.Object,
                generalWorkQueue.Object);
            return controller;
        }

        private Mock<IHttpContextAccessor> CreateRequestForExceptionTesting(FormFile file = null)
        {
            var httpContext = new DefaultHttpContext();

            FormFileCollection formFileCollection;
            if (file != null)
            {
                formFileCollection = new FormFileCollection { file };
            }
            else
            {
                formFileCollection = new FormFileCollection();
            }

            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(_ => _.HttpContext).Returns(httpContext);
            return accessor;
        }

        [Fact]
        public async Task GetInvestment_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            budgetRepo.Setup(b => b.GetScenarioBudgets(simulationId)).ReturnsEmptyList();
            var investmentPlan = InvestmentPlanDtos.Dto(Guid.Empty);
            investmentPlanRepo.Setup(i => i.GetInvestmentPlan(simulationId)).Returns(investmentPlan);

            // Act
            var result = await controller.GetInvestment(simulationId);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var expected = new InvestmentDTO
            {
                InvestmentPlan = new InvestmentPlanDTO
                {
                    FirstYearOfAnalysisPeriod = 2022,
                    InflationRatePercentage = 3,
                    NumberOfYearsInAnalysisPeriod = 1,
                    MinimumProjectCostLimit = 1000,
                },
                ScenarioBudgets = new List<BudgetDTO>(),
            };
            ObjectAssertions.Equivalent(expected, value);
        }

        [Fact]
        public async Task UpsertNewLibrary_Does()
        {
            var user = UserDtos.Admin();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = new BudgetLibraryDTO { Id = libraryId, Name = "Library", Budgets = new List<BudgetDTO>() };
            var request = new InvestmentLibraryUpsertPagingRequestModel();
            var libraryAccess = LibraryAccessModels.LibraryDoesNotExist();
            budgetRepo.Setup(br => br.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(libraryAccess);
            request.IsNewLibrary = true;
            request.Library = dto;
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.UpsertBudgetLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var createLibraryInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.CreateNewBudgetLibrary));
            var expectedUpsertedLibrary = new BudgetLibraryDTO
            {
                Id = libraryId,
                Budgets = new List<BudgetDTO>(),
                Name = "Library",
            };
            ObjectAssertions.Equivalent(expectedUpsertedLibrary, createLibraryInvocation.Arguments[0]);
            Assert.Equal(user.Id, createLibraryInvocation.Arguments[1]);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            budgetRepo.Setup(b => b.GetScenarioBudgets(simulationId)).ReturnsEmptyList();
            var controller = CreateController(unitOfWork);
            var request = new InvestmentPagingSyncModel();
            var investmentPlan = new InvestmentPlanDTO();
            request.Investment = investmentPlan;

            // Act
            var result = await controller.UpsertInvestment(simulationId, request);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteScenarioBudgetsWithInvestmentPlan));
            var budgets = invocation.Arguments[0] as List<BudgetDTO>;
            Assert.Empty(budgets);
            ObjectAssertions.Equivalent(invocation.Arguments[1], investmentPlan);
            Assert.Equal(simulationId, invocation.Arguments[2]);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();

            // Act
            var result = await controller.DeleteBudgetLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var repoCall = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
            Assert.Equal(libraryId, repoCall.Arguments[0]);
        }

        [Fact]
        public async Task GetBudgetLibraries_CallsGetBudgetLibrariesNoChildrenOnController()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = new BudgetLibraryDTO { Id = libraryId, Name = "Library", Budgets = new List<BudgetDTO>() };
            var request = new InvestmentLibraryUpsertPagingRequestModel();
            var libraryAccess = LibraryAccessModels.LibraryExistsWithUsers(Guid.Empty, null);
            budgetRepo.Setup(br => br.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(libraryAccess);
            request.IsNewLibrary = true;
            request.Library = dto;
            var controller = CreateController(unitOfWork);
            var library = BudgetLibraryDtos.New();
            budgetRepo.Setup(b => b.GetBudgetLibrariesNoChildren()).ReturnsList(library);

            // Act
            var result = await controller.GetBudgetLibraries();

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.CheckEnumerable(value, library);
        }

        [Fact]
        public async Task UpsertBudgetLibrary_LibraryExists_Modifies()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var libraryAccess = LibraryAccessModels.LibraryExistsWithUsers(Guid.Empty, null);
            var libraryId = Guid.NewGuid();
            budgetRepo.Setup(br => br.GetLibraryAccess(libraryId, Guid.Empty)).Returns(libraryAccess);
            var controller = CreateController(unitOfWork);
            var budgetId = Guid.NewGuid();
            var existingLibrary = BudgetLibraryDtos.New(libraryId);
            var existingBudget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2022, 1234m);
            existingLibrary.Budgets.Add(existingBudget);
            budgetRepo.Setup(b => b.GetBudgetLibrary(libraryId)).Returns(existingLibrary);
            var updatedLibrary = BudgetLibraryDtos.New(libraryId);
            var updatedBudget = BudgetDtos.WithSingleAmount(budgetId, "Budget", 2022, 1000000);
            updatedLibrary.Budgets.Add(updatedBudget);
            updatedLibrary.Description = "Updated Description";
            updatedLibrary.Budgets[0].CriterionLibrary = new CriterionLibraryDTO();

            var request = new InvestmentLibraryUpsertPagingRequestModel();

            request.Library = updatedLibrary;
            request.SyncModel.UpdatedBudgets.Add(updatedBudget);

            // Act
            await controller.UpsertBudgetLibrary(request);

            // Assert
            var invocations = budgetRepo.Invocations.ToList();
            var upsertLibraryInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpdateBudgetLibraryAndUpsertOrDeleteBudgets));
            var upsertedLibrary = upsertLibraryInvocation.Arguments[0] as BudgetLibraryDTO;
            Assert.Equal("Updated Description", upsertedLibrary.Description);
            var updatedBudgetDtos = upsertedLibrary.Budgets;
            var updatedBudgetDto = updatedBudgetDtos.Single();
            Assert.Equal(1234m, updatedBudgetDto.BudgetAmounts[0].Value); // counterintuitive behavior. Verified that it is present in the repo back to at least Sept. 30, 2022 (commit 82e8df57004)
            ObjectAssertions.EquivalentExcluding(updatedBudgetDto, updatedBudget, x => x.BudgetAmounts[0].Id, x => x.BudgetAmounts[0].Value);
            Assert.Equal(libraryId, upsertedLibrary.Id);
        }

        [Fact]
        public async Task ShouldModifyInvestmentData()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
            var budgetId = Guid.NewGuid();
            var budgetAmountId = Guid.NewGuid();
            var oldBudget = BudgetDtos.New(budgetId);
            var newBudget = BudgetDtos.New(budgetId, "Updated Name");
            var oldBudgetAmount = BudgetAmountDtos.ForBudgetAndYear(oldBudget, 2023, 500000, budgetAmountId);
            var newBudgetAmount = BudgetAmountDtos.ForBudgetAndYear(newBudget, 2023, 1000000, budgetAmountId);
            oldBudget.BudgetAmounts.Add(oldBudgetAmount);
            newBudget.BudgetAmounts.Add(newBudgetAmount);
            var investmentPlanId = Guid.NewGuid();
            var investmentPlan = InvestmentPlanDtos.Dto(investmentPlanId);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var oldBudgetLibrary = new BudgetLibraryDTO
            {
                Id = libraryId,
                Budgets = new List<BudgetDTO> { oldBudget }
            };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).ReturnsList(oldBudget);
            var controller = CreateController(unitOfWork);
            var dto = new InvestmentDTO
            {
                ScenarioBudgets = new List<BudgetDTO> { newBudget },
                InvestmentPlan = investmentPlan,
            };
            dto.ScenarioBudgets[0].CriterionLibrary = new CriterionLibraryDTO();
            dto.InvestmentPlan.MinimumProjectCostLimit = 1000000;
            var request = new InvestmentPagingSyncModel();
            request.Investment = dto.InvestmentPlan;
            request.UpdatedBudgets.Add(newBudget);
            request.UpdatedBudgetAmounts[dto.ScenarioBudgets[0].Name] = new List<BudgetAmountDTO>() { newBudgetAmount};

            // Act
            await controller.UpsertInvestment(simulationId, request);

            var upsertInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteScenarioBudgetsWithInvestmentPlan));
            Assert.Equal(simulationId, upsertInvocation.Arguments[2]);
            var modifiedBudgetDtos = upsertInvocation.Arguments[0] as List<BudgetDTO>;
            var modifiedBudgetDto = modifiedBudgetDtos.Single();
            Assert.Equal("Updated Name", modifiedBudgetDto.Name);
            Assert.Equal(dto.ScenarioBudgets[0].CriterionLibrary.Id,
                modifiedBudgetDto.CriterionLibrary.Id);
            Assert.Single(modifiedBudgetDto.BudgetAmounts);
            Assert.Equal(1000000,
               modifiedBudgetDto.BudgetAmounts[0].Value);
            var modifiedInvestmentPlan = upsertInvocation.Arguments[1] as InvestmentPlanDTO;
            Assert.Equal(1000000, modifiedInvestmentPlan.MinimumProjectCostLimit);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoMimeTypeForLibraryImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var service = CreateService(unitOfWork);
            var controller = CreateController(unitOfWork);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Request MIME type is invalid.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesForLibraryImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateController(unitOfWork, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetLibraryIdFoundForImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateController(unitOfWork, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no budget library id.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesForScenarioImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateController(unitOfWork, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetSimulationIdFoundForImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateController(unitOfWork, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no simulation id.", exception.Message);
        }
    }
}
