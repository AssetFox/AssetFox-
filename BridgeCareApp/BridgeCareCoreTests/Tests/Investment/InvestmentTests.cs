using System.Data;
using System.Security.Claims;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Investment;
using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using MoreLinq;
using OfficeOpenXml;
using Xunit;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentControllerTests
    {
        private BudgetLibraryEntity _testBudgetLibrary;
        private BudgetEntity _testBudget;
        private InvestmentPlanEntity _testInvestmentPlan;
        private ScenarioBudgetEntity _testScenarioBudget;
        private const string BudgetEntityName = "Budget";
        private readonly Mock<IInvestmentDefaultDataService> _mockInvestmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public InvestmentBudgetsService SetupDatabaseBasedService()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var hubService = HubServiceMocks.Default();
            var logger = new LogNLog();
            var service = new InvestmentBudgetsService(
                TestHelper.UnitOfWork,
                new ExpressionValidationService(TestHelper.UnitOfWork, logger),
                hubService,
                new InvestmentDefaultDataService());
            return service;
        }

        private InvestmentController CreateController(Mock<IUnitOfWork> unitOfWork, List<Claim> contextAccessorClaims, Mock<IHubService> hubServiceMock = null)
        {
            var resolveHubService = hubServiceMock ?? new Mock<IHubService>();
            var accessor = HttpContextAccessorMocks.WithClaims(contextAccessorClaims);
            var security = EsecSecurityMocks.Dbe;
            var mockDataService = _mockInvestmentDefaultDataService;
            var claimHelper = new ClaimHelper(unitOfWork.Object, accessor);
            var investmentBudgetServiceMock = InvestmentBudgetServiceMocks.New();
            var controller = new InvestmentController(
                investmentBudgetServiceMock.Object,
                security,
                unitOfWork.Object,
                resolveHubService.Object,
                accessor,
                mockDataService.Object,
                claimHelper);
            return controller;
        }

        private InvestmentController CreateAdminController(Mock<IUnitOfWork> unitOfWork, Mock<IHubService> hubServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Admin();
            var controller = CreateController(unitOfWork, claims, hubServiceMock);
            return controller;
        }

        private InvestmentController CreateNonAdminController(Mock<IUnitOfWork> unitOfWork, Mock<IHubService> hubServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Empty();
            var controller = CreateController(unitOfWork, claims, hubServiceMock);
            return controller;
        }

        private InvestmentController CreateDatabaseAuthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new InvestmentController(service, EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        private InvestmentController CreateDatabaseUnauthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new InvestmentController(service, EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        private void AddTestDataToDatabase()
        {
            var year = DateTime.Now.Year;
            _testBudgetLibrary = new BudgetLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetLibrary);
            TestHelper.UnitOfWork.Context.SaveChanges();

            _testBudget = new BudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = BudgetEntityName,
                BudgetLibraryId = _testBudgetLibrary.Id,
                BudgetAmounts =
                new List<BudgetAmountEntity>
                {
                   new BudgetAmountEntity {Id = Guid.NewGuid(), Year = year, Value = 500000}
                },
                CriterionLibraryBudgetJoin = new CriterionLibraryBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        MergedCriteriaExpression = "expression",
                        Name = "Criterion"
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void AddScenarioDataToDatabase(Guid simulationId)
        {
            var year = DateTime.Now.Year;
            _testInvestmentPlan = new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                FirstYearOfAnalysisPeriod = year,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
                InflationRatePercentage = 3
            };
            var investmentPlan = TestHelper.UnitOfWork.Context.InvestmentPlan.FirstOrDefault(ip => ip.SimulationId == simulationId);
            if (investmentPlan != null)
            {
                TestHelper.UnitOfWork.Context.Remove(investmentPlan);
            }
            TestHelper.UnitOfWork.Context.AddEntity(_testInvestmentPlan);

            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = BudgetEntityName,
                SimulationId = simulationId,
                ScenarioBudgetAmounts =
                        new List<ScenarioBudgetAmountEntity>
                        {
                        new ScenarioBudgetAmountEntity
                        {
                            Id = Guid.NewGuid(), Year = year, Value = 5000000
                        }
                        },
                CriterionLibraryScenarioBudgetJoin = new CriterionLibraryScenarioBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        MergedCriteriaExpression = "expression",
                        Name = "Criterion"
                    }
                }
            };
            var scenarioBudget = TestHelper.UnitOfWork.Context.ScenarioBudget.FirstOrDefault(b => b.Name == BudgetEntityName);
            if (scenarioBudget != null)
            {
                TestHelper.UnitOfWork.Context.Remove(scenarioBudget);
            }
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private IHttpContextAccessor CreateRequestWithLibraryFormData(bool overwriteBudgets = false)
        {
            var httpContext = new DefaultHttpContext();
            var formData = new Dictionary<string, StringValues>()
            {
                {"overwriteBudgets", overwriteBudgets ? new StringValues("1") : new StringValues("0")},
                {"libraryId", new StringValues(_testBudgetLibrary.Id.ToString())},
                {"currentUserCriteriaFilter", Newtonsoft.Json.JsonConvert.SerializeObject(new UserCriteriaDTO
                {
                    CriteriaId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    UserName = "Test User",
                    Criteria = string.Empty,
                    HasCriteria = false,
                    HasAccess = true,
                })}
            };
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestInvestmentBudgets.xlsx");
            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection { formFile });
            HttpContextSetup.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var mock = new Mock<IHttpContextAccessor>();
            mock.Setup(m => m.HttpContext).Returns(httpContext);
            return mock.Object;
        }

        private IHttpContextAccessor CreateRequestWithScenarioFormData(Guid simulationId, bool overwriteBudgets = false)
        {
            var httpContext = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestInvestmentBudgets.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"overwriteBudgets", overwriteBudgets ? new StringValues("1") : new StringValues("0")},
                {"simulationId", new StringValues(simulationId.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection { formFile });
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(_ => _.HttpContext).Returns(httpContext);
            return accessor.Object;
        }

        private Dictionary<string, string> GetCriteriaPerBudgetName()
        {
            var criteriaPerBudgetName = new Dictionary<string, string>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var excelPackage = new ExcelPackage(memStream);
            var criteriaWorksheet = excelPackage.Workbook.Worksheets[1];
            var budgetCol = 1;
            var criteriaCol = 2;
            for (var row = 2; row <= criteriaWorksheet.Dimension.End.Row; row++)
            {
                var budgetName = criteriaWorksheet.GetValue<string>(row, budgetCol);
                var criteria = criteriaWorksheet.GetValue<string>(row, criteriaCol);
                if (!criteriaPerBudgetName.ContainsKey(budgetName))
                {
                    criteriaPerBudgetName.Add(budgetName, string.Empty);
                }

                criteriaPerBudgetName[budgetName] = criteria;
            }

            return criteriaPerBudgetName;
        }

        private IHttpContextAccessor CreateRequestForExceptionTesting(FormFile file = null)
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
            return accessor.Object;
        }

        [Fact]
        public async Task RequestUpsertNewBudgetLibrary_ForwardsRequestToService()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateAdminController(unitOfWork);
            var library = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var pagingSync = new InvestmentPagingSyncModel
            {
                
            };
            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
            {
                IsNewLibrary = true,
                Library = library,
                PagingSync = pagingSync,
            };
            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
            var upsertCall = upsertCalls.Single();
            var actualDto = upsertCall.Item1;
            ObjectAssertions.Equivalent(library, actualDto);
        }

        [Fact]
        public async Task RequestUpsertExistingBudgetLibrary_ForwardsRequestToService()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Owner);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateAdminController(unitOfWork);
            var library = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var pagingSync = new InvestmentPagingSyncModel
            {

            };
            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
            {
                IsNewLibrary = false,
                Library = library,
                PagingSync = pagingSync,
            };
            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
            var upsertCall = upsertCalls.Single();
            var actualDto = upsertCall.Item1;
            ObjectAssertions.Equivalent(library, actualDto);
            Assert.True(upsertCall.Item2);
        }

        [Fact]
        public async Task RequestUpsertExistingBudgetLibrary_UserIsAdminWithoutExplicitAccess_ForwardsRequestToService()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateAdminController(unitOfWork);
            var library = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var pagingSync = new InvestmentPagingSyncModel
            {

            };
            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
            {
                IsNewLibrary = false,
                Library = library,
                PagingSync = pagingSync,
            };
            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
            var upsertCall = upsertCalls.Single();
            var actualDto = upsertCall.Item1;
            ObjectAssertions.Equivalent(library, actualDto);
            Assert.True(upsertCall.Item2);
        }

        [Fact]
        public async Task RequestUpsertExistingBudgetLibrary_UserIsNotAdminWithoutExplicitAccess_DoesNotForwardRequestToService()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = new Mock<IHubService>();
            var controller = CreateNonAdminController(unitOfWork, hubService);
            var library = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var pagingSync = new InvestmentPagingSyncModel
            {

            };
            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
            {
                IsNewLibrary = false,
                Library = library,
                PagingSync = pagingSync,
            };
            await controller.UpsertBudgetLibrary(upsertRequest);
            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
            Assert.Empty(upsertCalls);
            var messages = hubService.ThreeArgumentUserMessages();
            var message = messages.Single();
            Assert.Contains(ClaimHelper.LibraryModifyUnauthorizedMessage, message);
        }

        [Fact]
        public async Task DeleteBudgetLibrary_LibraryDoesNotExistAdminUser_UnauthorizedAndDoesNotCallDeleteOnRepo()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = CreateAdminController(unitOfWork, hubService);

            var result = await controller.DeleteBudgetLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            budgetRepo.Verify(br => br.DeleteBudgetLibrary(libraryId), Times.Once());
            budgetRepo.Verify(br => br.DeleteBudgetLibrary(It.IsAny<Guid>()), Times.Once());
            var message = hubService.SingleThreeArgumentUserMessage();
            Assert.Contains(ClaimHelper.CantDeleteNonexistentLibraryMessage, message);
        }

        [Fact]
        public async Task DeleteBudgetLibrary_LibraryDoesNotExistNonAdminUser_UnauthorizedAndDoesNotCallDeleteOnRepo()
        {
            var user = UserDtos.Dbe();
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = CreateNonAdminController(unitOfWork, hubService);

            var result = await controller.DeleteBudgetLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            budgetRepo.Verify(br => br.DeleteBudgetLibrary(It.IsAny<Guid>()), Times.Never());
            var message = hubService.SingleThreeArgumentUserMessage();
            Assert.Contains(ClaimHelper.CantDeleteNonexistentLibraryMessage, message);
        }

        [Fact]
        public async Task DeleteBudgetLibrary_AdminUser_Ok()
        {
            var user = UserDtos.Admin;
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateAdminController(unitOfWork);

            var result = await controller.DeleteBudgetLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
        }

        [Fact]
        public async Task DeleteBudgetLibrary_OwnerButNotAdminUser_Ok()
        {
            var user = UserDtos.Dbe();
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Owner);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateNonAdminController(unitOfWork);

            var result = await controller.DeleteBudgetLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
            Assert.Single(calls);
        }

        [Fact]
        public async Task DeleteBudgetLibrary_ModifyPermissionButNotAdminUser_DoesNotDelete()
        {
            var user = UserDtos.Dbe();
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Modify);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = CreateNonAdminController(unitOfWork, hubService);

            var result = await controller.DeleteBudgetLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
            Assert.Empty(calls);
            var messages = hubService.ThreeArgumentUserMessages();
            var errorMessage = messages.Single();
            Assert.Contains(ClaimHelper.LibraryDeleteUnauthorizedMessage, errorMessage);
        }

        [Fact]
        public async Task GetBudgetLibraries_UserIsAdmin_CallsGetLibrariesWithoutChildrenOnRepo()
        {
            var user = UserDtos.Admin;
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var budgetRepo = BudgetRepositoryMocks.New();
            var libraryId = Guid.NewGuid();
            var budgetLibrary = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var budgetLibraries = new List<BudgetLibraryDTO> { budgetLibrary };
            budgetRepo.Setup(br => br.GetBudgetLibrariesNoChildren()).Returns(budgetLibraries);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateAdminController(unitOfWork);

            var libraries = await controller.GetBudgetLibraries();

            var returnedBudgetLibraries = ActionResultAssertions.OkObject(libraries) as List<BudgetLibraryDTO>;
            var returnedBudgetLibrary = returnedBudgetLibraries.Single();
            ObjectAssertions.Equivalent(budgetLibrary, returnedBudgetLibrary);
        }

        [Fact]
        public async Task GetBudgetLibraries_UserIsNotAdmin_CallsGetBudgetLibrariesNoChildrenAccessibleToUser()
        {
            var user = UserDtos.Dbe();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var budgetRepo = BudgetRepositoryMocks.New();
            var libraryId = Guid.NewGuid();
            var budgetLibrary = new BudgetLibraryDTO
            {
                Id = libraryId,
            };
            var budgetLibraries = new List<BudgetLibraryDTO> { budgetLibrary };
            budgetRepo.Setup(br => br.GetBudgetLibrariesNoChildrenAccessibleToUser(user.Id)).Returns(budgetLibraries);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var controller = CreateNonAdminController(unitOfWork);

            var libraries = await controller.GetBudgetLibraries();

            var returnedBudgetLibraries = ActionResultAssertions.OkObject(libraries) as List<BudgetLibraryDTO>;
            var returnedBudgetLibrary = returnedBudgetLibraries.Single();
            ObjectAssertions.Equivalent(budgetLibrary, returnedBudgetLibrary);
        }

        [Fact]
        public async Task GetInvestment_UserIsAdmin_BudgetRepoAndInvestmentPlanRepo()
        {
            var user = UserDtos.Admin;
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var budgetRepo = BudgetRepositoryMocks.New();
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budget = new BudgetDTO
            {
                Id = budgetId,
            };
            var budgets = new List<BudgetDTO> { budget };
            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock();
            unitOfWork.SetupInvestmentPlanRepo(investmentPlanRepo);
            var investmentPlanId = Guid.NewGuid();
            var investmentPlanDto = new InvestmentPlanDTO
            {
                Id = investmentPlanId,
            };
            investmentPlanRepo.Setup(r => r.GetInvestmentPlan(simulationId)).Returns(investmentPlanDto);
            var controller = CreateAdminController(unitOfWork);

            var libraries = await controller.GetInvestment(simulationId);

            var returnedInvestmentDto = ActionResultAssertions.OkObject(libraries) as InvestmentDTO;
            var expected = new InvestmentDTO
            {
                InvestmentPlan = investmentPlanDto,
                ScenarioBudgets = budgets,
            };
            ObjectAssertions.Equivalent(expected, returnedInvestmentDto);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            var service = SetupDatabaseBasedService();
            // Arrange
            var controller = CreateDatabaseAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            var request = new InvestmentPagingSyncModel();
            request.Investment = new InvestmentPlanDTO();


            // Act
            var result = await controller.UpsertInvestment(simulation.Id, request);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var service = SetupDatabaseBasedService();
            // Arrange
            var controller = CreateDatabaseAuthorizedController(service);

            // Act
            var result = await controller.DeleteBudgetLibrary(Guid.Empty);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldGetLibraryDataNoChildren()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);
            AddTestDataToDatabase();

            // Act
            var result = await controller.GetBudgetLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<BudgetLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<BudgetLibraryDTO>));
            Assert.Contains(dtos, b => b.Id == _testBudgetLibrary.Id);
            var resultBudgetLibrary = dtos.FirstOrDefault(b => b.Id == _testBudgetLibrary.Id);

            Assert.True(resultBudgetLibrary.Budgets.Count == 0);
        }

        [Fact]
        public async Task ShouldGetInvestmentData()
        {
            var service = SetupDatabaseBasedService();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateDatabaseAuthorizedController(service);
            AddScenarioDataToDatabase(simulation.Id);

            // Act
            var result = await controller.GetInvestment(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
            Assert.Single(dto.ScenarioBudgets);
            Assert.Equal(_testInvestmentPlan.FirstYearOfAnalysisPeriod,
                dto.InvestmentPlan.FirstYearOfAnalysisPeriod);
            Assert.Equal(_testInvestmentPlan.MinimumProjectCostLimit, dto.InvestmentPlan.MinimumProjectCostLimit);
            Assert.Equal(_testInvestmentPlan.NumberOfYearsInAnalysisPeriod,
                dto.InvestmentPlan.NumberOfYearsInAnalysisPeriod);

            Assert.Single(dto.ScenarioBudgets[0].BudgetAmounts);
            var budgetAmount = _testScenarioBudget.ScenarioBudgetAmounts.ToList()[0];
            var dtoBudgetAmount = dto.ScenarioBudgets[0].BudgetAmounts[0];
            Assert.Equal(budgetAmount.Id, dtoBudgetAmount.Id);
            Assert.Equal(budgetAmount.Year, dtoBudgetAmount.Year);
            Assert.Equal(budgetAmount.Value, dtoBudgetAmount.Value);

            Assert.Equal(_testScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibraryId,
                dto.ScenarioBudgets[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldModifyLibraryData()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);
            AddTestDataToDatabase();

            _testBudgetLibrary.Budgets = new List<BudgetEntity> { _testBudget };
            var dto = _testBudgetLibrary.ToDto();
            dto.Description = "Updated Description";
            dto.Budgets[0].Name = "Updated Name";
            dto.Budgets[0].BudgetAmounts[0].Value = 1000000;
            dto.Budgets[0].CriterionLibrary = new CriterionLibraryDTO();

            var request = new InvestmentLibraryUpsertPagingRequestModel();

            request.Library = dto;
            request.PagingSync.UpdatedBudgets.Add(dto.Budgets[0]);

            // Act
            await controller.UpsertBudgetLibrary(request);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibraries().Single(lib => lib.Id == dto.Id);

            Assert.Equal(dto.Description, modifiedDto.Description);

            Assert.Equal(dto.Budgets[0].Name, modifiedDto.Budgets[0].Name);
            Assert.Equal(dto.Budgets[0].CriterionLibrary.Id,
                modifiedDto.Budgets[0].CriterionLibrary.Id);

            Assert.Single(modifiedDto.Budgets[0].BudgetAmounts);
            Assert.Equal(dto.Budgets[0].BudgetAmounts[0].Value,
                modifiedDto.Budgets[0].BudgetAmounts[0].Value);
        }

        [Fact]
        public async Task ShouldModifyInvestmentData()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            AddScenarioDataToDatabase(simulation.Id);

            var dto = new InvestmentDTO
            {
                ScenarioBudgets = new List<BudgetDTO> { _testScenarioBudget.ToDto() },
                InvestmentPlan = _testInvestmentPlan.ToDto()
            };
            dto.ScenarioBudgets[0].Name = "Updated Name";
            dto.ScenarioBudgets[0].BudgetAmounts[0].Value = 1000000;
            dto.ScenarioBudgets[0].CriterionLibrary = new CriterionLibraryDTO();
            dto.InvestmentPlan.MinimumProjectCostLimit = 1000000;

            var request = new InvestmentPagingSyncModel();
            request.Investment = dto.InvestmentPlan;
            request.UpdatedBudgets.Add(dto.ScenarioBudgets[0]);
            request.UpdatedBudgetAmounts[dto.ScenarioBudgets[0].Name] = new List<BudgetAmountDTO>() { dto.ScenarioBudgets[0].BudgetAmounts[0] };

            // Act
            await controller.UpsertInvestment(simulation.Id, request);

            // Assert
            var modifiedBudgetDto =
                TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id)[0];
            var modifiedInvestmentPlanDto =
                TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulation.Id);

            Assert.Equal(dto.ScenarioBudgets[0].Name, modifiedBudgetDto.Name);
            Assert.Equal(dto.ScenarioBudgets[0].CriterionLibrary.Id,
                modifiedBudgetDto.CriterionLibrary.Id);

            Assert.Single(modifiedBudgetDto.BudgetAmounts);
            Assert.Equal(dto.ScenarioBudgets[0].BudgetAmounts[0].Value,
                modifiedBudgetDto.BudgetAmounts[0].Value);

            Assert.Equal(dto.InvestmentPlan.MinimumProjectCostLimit,
                modifiedInvestmentPlanDto.MinimumProjectCostLimit);
        }

        [Fact]
        public async Task ShouldDeleteBudgetData()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);
            AddTestDataToDatabase();

            // Act
            var result = await controller.DeleteBudgetLibrary(_testBudgetLibrary.Id);

            // Assert
            ActionResultAssertions.Ok(result);

            Assert.True(!TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == _testBudgetLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == _testBudget.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
                    _.BudgetId == _testBudget.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.BudgetAmount.Any(_ =>
                    _.Id == _testBudget.BudgetAmounts.ToList()[0].Id));
        }

        /**************************INVESTMENT BUDGETS EXCEL FILE IMPORT/EXPORT TESTS***********************************/
        [Fact]
        public async Task ShouldImportLibraryBudgetsFromFile()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            AddTestDataToDatabase();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateDatabaseAuthorizedController(service, accessor);
            var year = DateTime.Now.Year;


            // Act
            await controller.ImportLibraryInvestmentBudgetsExcelFile();

            // Assert
            var budgetAmounts = TestHelper.UnitOfWork.BudgetAmountRepo
                .GetLibraryBudgetAmounts(_testBudgetLibrary.Id)
                .Where(_ => _.Budget.Name.IndexOf("Sample") != -1)
                .ToList();

            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
            Assert.Equal(3, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
        }

        [Fact]
        public async Task ShouldOverwriteExistingLibraryBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = SetupDatabaseBasedService();
            AddTestDataToDatabase();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            _testBudget.Name = "Sample Budget 1";
            TestHelper.UnitOfWork.Context.UpdateEntity(_testBudget, _testBudget.Id);

            var budgetAmount = _testBudget.BudgetAmounts.ToList()[0];
            budgetAmount.Year = year;
            budgetAmount.Value = 4000000;
            TestHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);

            // Act
            await controller.ImportLibraryInvestmentBudgetsExcelFile();

            // Assert
            var budgetAmounts =
                TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(_testBudgetLibrary.Id);
            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
            Assert.Equal(2, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
            var criteria = TestHelper.UnitOfWork.Context.CriterionLibrary.AsNoTracking().AsSplitQuery()
                .Where(_ => _.IsSingleUse &&
                            _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                                budgetNames.Contains(join.ScenarioBudget.Name)))
                .Include(_ => _.CriterionLibraryScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ToList();
        }

        [Fact]
        public async Task ShouldExportSampleLibraryBudgetsFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = SetupDatabaseBasedService();
            AddTestDataToDatabase();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateDatabaseAuthorizedController(service, accessor);
            TestHelper.UnitOfWork.Context.DeleteAll<BudgetAmountEntity>(_ => _.BudgetId == _testBudget.Id);

            // Act
            var result =
                await controller.ExportLibraryInvestmentBudgetsExcelFile(_testBudgetLibrary.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal("sample_investment_budgets_import_export_file.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Equal(4, worksheetBudgetNames.Count);
            Assert.True(worksheetBudgetNames.All(name => name.Contains("Sample Budget")));

            var expectedYear = year;
            worksheet.Cells[2, 1, worksheet.Dimension.End.Row, 1]
                .Select(cell => cell.GetValue<int>()).ToList().ForEach(year =>
                {
                    Assert.Equal(expectedYear, year);
                    expectedYear++;
                });

            var budgetAmounts = worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<decimal>()).ToList();
            Assert.True(budgetAmounts.All(amount => amount == decimal.Parse("5000000")));
        }

        [Fact]
        public async Task ShouldExportLibraryBudgetsFile()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            AddTestDataToDatabase();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateDatabaseAuthorizedController(service, accessor);
            var year = DateTime.Now.Year;

            // Act
            var result =
                await controller.ExportLibraryInvestmentBudgetsExcelFile(_testBudgetLibrary.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal("Test_Name_investment_budgets.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Single(worksheetBudgetNames);
            Assert.Equal(_testBudget.Name, worksheetBudgetNames[0]);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Equal(year.ToString(), worksheetBudgetYearAndAmount[0]);
            Assert.Equal(_testBudget.BudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoMimeTypeForLibraryImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Request MIME type is invalid.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesForLibraryImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetLibraryIdFoundForImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no budget library id.", exception.Message);
        }

        [Fact]
        public async Task ShouldImportScenarioBudgetsFromFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = SetupDatabaseBasedService();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateDatabaseAuthorizedController(service, accessor);
            AddScenarioDataToDatabase(simulation.Id);

            // Act
            await controller.ImportScenarioInvestmentBudgetsExcelFile();

            // Assert
            var budgetAmounts = TestHelper.UnitOfWork.BudgetAmountRepo
                .GetScenarioBudgetAmounts(simulation.Id)
                .Where(_ => _.ScenarioBudget.Name.IndexOf("Sample") != -1)
                .ToList();

            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            Assert.Equal(3, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
        }

        [Fact]
        public async Task ShouldOverwriteExistingScenarioBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = SetupDatabaseBasedService();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            AddScenarioDataToDatabase(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            _testScenarioBudget.Name = "Sample Budget 1";
            TestHelper.UnitOfWork.Context.UpdateEntity(_testScenarioBudget, _testScenarioBudget.Id);

            var budgetAmount = _testScenarioBudget.ScenarioBudgetAmounts.ToList()[0];
            budgetAmount.Year = year;
            budgetAmount.Value = 4000000;
            TestHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);

            // Act
            await controller.ImportScenarioInvestmentBudgetsExcelFile();

            // Assert
            var budgetAmounts =
                TestHelper.UnitOfWork.BudgetAmountRepo.GetScenarioBudgetAmounts(simulation.Id);
            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            Assert.Equal(2, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
            var criteria = TestHelper.UnitOfWork.Context.CriterionLibrary.AsNoTracking().AsSplitQuery()
                .Where(_ => _.IsSingleUse &&
                            _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                                budgetNames.Contains(join.ScenarioBudget.Name)))
                .Include(_ => _.CriterionLibraryScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ToList();
        }

        [Fact]
        public async Task ShouldExportSampleScenarioBudgetsFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = SetupDatabaseBasedService();
            var simulationName = RandomStrings.Length11();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, null, simulationName);
            AddScenarioDataToDatabase(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            // Act
            var result =
                await controller.ExportScenarioInvestmentBudgetsExcelFile(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal($"{simulationName}_investment_budgets.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.True(worksheetBudgetNames.Count >= 1);
            Assert.True(worksheetBudgetNames.All(name => name.Contains(BudgetEntityName)));

            var expetedYear = year;
            worksheet.Cells[2, 1, worksheet.Dimension.End.Row, 1]
                .Select(cell => cell.GetValue<int>()).ToList().ForEach(year =>
                {
                    Assert.Equal(expetedYear, year);
                    year++;
                });

            var budgetAmounts = worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<decimal>()).ToList();
            Assert.Contains(budgetAmounts, amount => amount == decimal.Parse("5000000"));
        }

        [Fact]
        public async Task ShouldExportScenarioBudgetsFile()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var simulationName = RandomStrings.Length11();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, null, simulationName);
            AddScenarioDataToDatabase(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateDatabaseAuthorizedController(service, accessor);
            // Act
            var result =
                await controller.ExportScenarioInvestmentBudgetsExcelFile(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal($"{simulationName}_investment_budgets.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.True(worksheetBudgetNames.Any());
            Assert.Equal(_testScenarioBudget.Name, worksheetBudgetNames[0]);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            //  Assert.Equal(expectedYear.ToString(), worksheetBudgetYearAndAmount[0]);
            Assert.Equal(_testScenarioBudget.ScenarioBudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoMimeTypeForScenarioImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var controller = CreateDatabaseAuthorizedController(service);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Request MIME type is invalid.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesForScenarioImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetSimulationIdFoundForImport()
        {
            // Arrange
            var service = SetupDatabaseBasedService();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateDatabaseAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no simulation id.", exception.Message);
        }
    }
}
