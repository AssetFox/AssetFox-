using System.Data;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
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
using BridgeCareCore.Services.Paging;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using MoreLinq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentTests
    {
        private BudgetLibraryEntity _testBudgetLibrary;
        private BudgetEntity _testBudget;
        private InvestmentPlanEntity _testInvestmentPlan;
        private ScenarioBudgetEntity _testScenarioBudget;
        private const string BudgetEntityName = "Budget";
        private readonly Mock<IInvestmentDefaultDataService> _mockInvestmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public InvestmentBudgetsService Setup(Mock<IHubService> hubServiceMock = null)
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var hubService = hubServiceMock ?? HubServiceMocks.DefaultMock();
            var logger = new LogNLog();
            var service = new InvestmentBudgetsService(
                TestHelper.UnitOfWork,
                new ExpressionValidationService(TestHelper.UnitOfWork, logger),
                hubService.Object,
                new InvestmentDefaultDataService());
            return service;
        }

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
            var controller = new InvestmentController(
                service,
                pagingService,
                security,
                mockUnitOfWork.Object,
                hubService.Object,
                resolveAccessor.Object,
                dataService,
                claimHelper.Object);
            return controller;
        }

        private InvestmentController CreateAuthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null, Mock<IHubService> hubServiceMock = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var hubService = hubServiceMock ?? HubServiceMocks.DefaultMock();
            var pagingService = new InvestmentPagingService(TestHelper.UnitOfWork, new InvestmentDefaultDataService());
            var controller = new InvestmentController(service,
                pagingService,
                EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService.Object,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        private void CreateLibraryTestData()
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
                        Name = "Criterion",
                        IsSingleUse = true,
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData(Guid simulationId)
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
                        Name = "Criterion",
                        IsSingleUse = true,
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

        private IHttpContextAccessor CreateRequestWithLibraryFormDataWithExtraCriterion(bool overwriteBudgets = false)
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
                "TestInvestmentBudgetsWithExtraBudgetCriterion.xlsx");
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

        private IHttpContextAccessor CreateRequestWithScenarioFormDataWithExtraCriterion(Guid simulationId, bool overwriteBudgets = false)
        {
            var httpContext = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgetsWithExtraBudgetCriterion.xlsx");
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
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
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
            var upsertLibraryInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertBudgetLibrary));
            var upsertBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteBudgets));
            var upsertUsersInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
            ObjectAssertions.EmptyEnumerable<BudgetDTO>(upsertBudgetsInvocation.Arguments[0]);
            var expectedUpsertedLibrary = new BudgetLibraryDTO
            {
                Id = libraryId,
                Budgets = new List<BudgetDTO>(),
                Name = "Library",
            };
            ObjectAssertions.Equivalent(expectedUpsertedLibrary, upsertLibraryInvocation.Arguments[0]);
            Assert.Equal(libraryId, upsertBudgetsInvocation.Arguments[1]);
            var expectedUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
            };
            Assert.Equal(libraryId, upsertUsersInvocation.Arguments[0]);
            ObjectAssertions.CheckEnumerable(upsertUsersInvocation.Arguments[1], expectedUser);
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
            var upsertCall = investmentPlanRepo.SingleInvocationWithName(nameof(IInvestmentPlanRepository.UpsertInvestmentPlan));
            ObjectAssertions.Equivalent(upsertCall.Arguments[0], investmentPlan);
            Assert.Equal(simulationId, upsertCall.Arguments[1]);
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
        public async Task ShouldGetInvestmentData()
        {
            var service = Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedController(service);
            CreateScenarioTestData(simulation.Id);

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
            var upsertLibraryInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertBudgetLibrary));
            var budgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteBudgets));
            var upsertedLibrary = upsertLibraryInvocation.Arguments[0] as BudgetLibraryDTO;
            Assert.Equal("Updated Description", upsertedLibrary.Description);
            var updatedBudgetDtos = budgetsInvocation.Arguments[0] as List<BudgetDTO>;
            var updatedBudgetDto = updatedBudgetDtos.Single();
            Assert.Equal(1234m, updatedBudgetDto.BudgetAmounts[0].Value); // counterintuitive behavior. Verified that it is present in the repo back to at least Sept. 30, 2022 (commit 82e8df57004)
            ObjectAssertions.EquivalentExcluding(updatedBudgetDto, updatedBudget, x => x.BudgetAmounts[0].Id, x => x.BudgetAmounts[0].Value);
            Assert.Equal(libraryId, budgetsInvocation.Arguments[1]);
        }

        [Fact]
        public async Task ShouldModifyInvestmentData()
        {
            // Arrange
            var service = Setup();
            var controller = CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

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
            request.UpdatedBudgetAmounts[dto.ScenarioBudgets[0].Name] = new List<BudgetAmountDTO>() { dto.ScenarioBudgets[0].BudgetAmounts[0]};

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
            var service = Setup();
            var controller = CreateAuthorizedController(service);
            CreateLibraryTestData();

            // Act
            var result = await controller.DeleteBudgetLibrary(_testBudgetLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == _testBudgetLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == _testBudget.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
                    _.BudgetId == _testBudget.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.BudgetAmount.Any(_ =>
                    _.Id == _testBudget.BudgetAmounts.ToList()[0].Id));
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoMimeTypeForLibraryImport()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var service = CreateService(unitOfWork);
            var controller = CreateAuthorizedController(service);

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
