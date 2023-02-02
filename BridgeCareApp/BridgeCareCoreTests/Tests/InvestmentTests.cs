using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Moq;
using OfficeOpenXml;
using Xunit;
using MoreLinq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using BridgeCareCore.Logging;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCore.Models;
using BridgeCareCore.Utils.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCore.Utils;
using Microsoft.AspNetCore.Authorization;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.Paging;

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

        public InvestmentBudgetsService Setup()
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

        private InvestmentController CreateAuthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var pagingService = new InvestmentPagingService(TestHelper.UnitOfWork, new InvestmentDefaultDataService());
            var hubService = HubServiceMocks.Default();
            var controller = new InvestmentController(service,
                pagingService,
                EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        private InvestmentController CreateUnauthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new InvestmentController(service, new InvestmentPagingService(TestHelper.UnitOfWork, new InvestmentDefaultDataService()), EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        private InvestmentController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimName in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimName);
                claims.Add(claim);
            }
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var service = Setup();
            var controller = new InvestmentController(service, new Mock<IInvestmentPagingService>().Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork, hubService, accessor, _mockInvestmentDefaultDataService.Object, _mockClaimHelper.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
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
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            var service = Setup();
            // Arrange
            var controller = CreateAuthorizedController(service);

            // Act
            var result = await controller.GetBudgetLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            var service = Setup();
            // Arrange
            var controller = CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            var result = await controller.GetInvestment(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            var service = Setup();
            // Arrange
            var controller = CreateAuthorizedController(service);
            var dto = new BudgetLibraryDTO { Id = Guid.NewGuid(), Name = "", Budgets = new List<BudgetDTO>() };

            var request = new InvestmentLibraryUpsertPagingRequestModel();
            request.IsNewLibrary = true;

            request.Library = dto;

            // Act
            var result = await controller.UpsertBudgetLibrary(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            var service = Setup();
            // Arrange
            var controller = CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            var request = new InvestmentPagingSyncModel();
            request.Investment = new InvestmentPlanDTO();
            

            // Act
            var result = await controller.UpsertInvestment(simulation.Id, request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var service = Setup();
            // Arrange
            var controller = CreateAuthorizedController(service);

            // Act
            var result = await controller.DeleteBudgetLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryDataNoChildren()
        {
            // Arrange
            var service = Setup();
            var controller = CreateAuthorizedController(service);
            CreateLibraryTestData();

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
        public async Task ShouldModifyLibraryData()
        {
            // Arrange
            var service = Setup();
            var controller = CreateAuthorizedController(service);
            CreateLibraryTestData();

            _testBudgetLibrary.Budgets = new List<BudgetEntity> { _testBudget };
            var dto = _testBudgetLibrary.ToDto();
            dto.Description = "Updated Description";
            dto.Budgets[0].Name = "Updated Name";
            dto.Budgets[0].BudgetAmounts[0].Value = 1000000;
            dto.Budgets[0].CriterionLibrary = new CriterionLibraryDTO();

            var request = new InvestmentLibraryUpsertPagingRequestModel();

            request.Library = dto;
            request.SyncModel.UpdatedBudgets.Add(dto.Budgets[0]);

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
        public async Task UserIsViewInvestmentFromScenarioAuthorized()
        {
            // admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewInvestmentFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.InvestmentViewAnyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewInvestmentFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyInestmentFromLibraryAuthorized()
        {
            // non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyInvestmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.InvestmentModifyPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyInvestmentFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);

        }
        [Fact]
        public async Task UserIsImportInvestmentFromScenarioAuthorized()
        {
            // non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ImportInvestmentFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.InvestmentImportPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ImportInvestmentFromScenario);
            // Assert
            Assert.False(allowed.Succeeded);

        }
        [Fact]
        public async Task UserIsViewInvestmentFromScenarioAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewInvestmentFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.InvestmentViewAnyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewInvestmentFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        /**************************INVESTMENT BUDGETS EXCEL FILE IMPORT/EXPORT TESTS***********************************/
        [Fact]
        public async Task ShouldImportLibraryBudgetsFromFile()
        {
            // Arrange
            var service = Setup();
            CreateLibraryTestData();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateAuthorizedController(service, accessor);
            var year = 2022;


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
            var year = 2022;
            var service = Setup();
            CreateLibraryTestData();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateAuthorizedController(service, accessor);

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
            var service = Setup();
            CreateLibraryTestData();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateAuthorizedController(service, accessor);
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
            var service = Setup();
            CreateLibraryTestData();
            var accessor = CreateRequestWithLibraryFormData();
            var controller = CreateAuthorizedController(service, accessor);
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
            var service = Setup();
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
            var service = Setup();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetLibraryIdFoundForImport()
        {
            // Arrange
            var service = Setup();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportLibraryInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no budget library id.", exception.Message);
        }

        [Fact]
        public async Task ShouldImportScenarioBudgetsFromFile()
        {
            // Arrange
            var year = 2022;
            var service = Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateAuthorizedController(service, accessor);
            CreateScenarioTestData(simulation.Id);

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

        [Fact(Skip = "WJ needs a working front end to investigate this failure, which arose over the new year -- but that may not be the whole issue with it")]
        public async Task ShouldOverwriteExistingScenarioBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            // Arrange
            var year = 2022;
            var service = Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateAuthorizedController(service, accessor);

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
            var service = Setup();
            var simulationName = RandomStrings.Length11();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, name: simulationName);
            CreateScenarioTestData(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateAuthorizedController(service, accessor);

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
            var service = Setup();
            var simulationName = RandomStrings.Length11();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, null, simulationName);
            CreateScenarioTestData(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            var controller = CreateAuthorizedController(service, accessor);
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
            var service = Setup();
            var controller = CreateAuthorizedController(service);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Request MIME type is invalid.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesForScenarioImport()
        {
            // Arrange
            var service = Setup();
            var accessor = CreateRequestForExceptionTesting();
            var controller = CreateAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Investment budgets file not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoBudgetSimulationIdFoundForImport()
        {
            // Arrange
            var service = Setup();
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            var accessor = CreateRequestForExceptionTesting(file);
            var controller = CreateAuthorizedController(service, accessor);

            // Act + Asset
            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                await controller.ImportScenarioInvestmentBudgetsExcelFile());
            Assert.Equal("Request contained no simulation id.", exception.Message);
        }
    }
}
