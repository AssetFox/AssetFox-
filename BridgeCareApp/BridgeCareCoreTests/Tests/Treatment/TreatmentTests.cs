using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Treatment;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCore.Interfaces;

namespace BridgeCareCoreTests.Tests
{
    public class TreatmentTests
    {
        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        private TreatmentController CreateAuthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new TreatmentController(TreatmentServiceMocks.EmptyMock.Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            return controller;
        }

        private TreatmentController CreateAuthorizedControllerWithTreatmService()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var treatmentService = new TreatmentService(TestHelper.UnitOfWork, new ExcelTreatmentLoader(new Mock<IExpressionValidationService>().Object));
            var controller = new TreatmentController(treatmentService, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            return controller;
        }


        private TreatmentController CreateUnauthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new TreatmentController(TreatmentServiceMocks.EmptyMock.Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            return controller;
        }

        private TreatmentController CreateTestController(List<string> userClaims)
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
            var controller = new TreatmentController(TreatmentServiceMocks.EmptyMock.Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private ScenarioBudgetEntity CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            TestHelper.UnitOfWork.Context.AddEntity(budget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            TestHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = budget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            TestHelper.UnitOfWork.Context.SaveChanges();
            return budget;
        }

        [Fact]
        public async Task ShouldGetSelectedTreatmentByIdWithDataUnfixable()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.GetSelectedTreatmentById(_testTreatment.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (TreatmentDTO)Convert.ChangeType(okObjResult.Value,
                typeof(TreatmentDTO));        

            Assert.Equal(_testTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);

            Assert.Equal(_testTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, dto.Costs[0].Id);
        }
        [Fact]
        public async Task ShouldGetScenarioSelectedTreatmentByIdWithDataUnfixable()
        {
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedController();
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var result = await controller.GetScenarioSelectedTreatmentById(_testScenarioTreatment.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (TreatmentDTO)Convert.ChangeType(okObjResult.Value, typeof(TreatmentDTO));

            Assert.Equal(_testScenarioTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);
            Assert.Single(dto.BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dto.Costs[0].Id);
            Assert.Contains(budget.Id, dto.BudgetIds);
        }


        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            //Setup();
            //// Act
            //var controller = CreateAuthorizedController();
            //var result = await controller.GetTreatmentLibraries();

            //// Assert
            //Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            //Setup();
            //var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            //// Act
            //var controller = CreateAuthorizedController();
            //var result = await controller.GetScenarioSelectedTreatments(simulation.Id);

            //// Assert
            //Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            //Setup();
            //var controller = CreateAuthorizedControllerWithTreatmService();
            //var dto = new TreatmentLibraryDTO
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "",
            //    Treatments = new List<TreatmentDTO>()
            //};

            //var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            //{
            //    IsNewLibrary = true,
            //    Library = dto,
            //};

            //// Act
            //var result = await controller.UpsertTreatmentLibrary(libraryRequest);

            // Assert
          //  Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
        //    // Arrange
        //    Setup();
        //    var controller = CreateAuthorizedControllerWithTreatmService();
        //    var dtos = new List<TreatmentDTO>();
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

        //    var pageSync = new PagingSyncModel<TreatmentDTO>();

        //    // Act
        //    var result = await controller.UpsertScenarioSelectedTreatments(simulation.Id, pageSync);

        //    // Assert
        //    Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryDelete()
        {
            //// Arrange
            //Setup();
            //var controller = CreateAuthorizedController();

            //// Act
            //var result = await controller.DeleteTreatmentLibrary(Guid.Empty);

            //// Assert
            //Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryTreatmentData()
        {
            //// Arrange
            //Setup();
            //var controller = CreateAuthorizedController();
            //CreateLibraryTestData();

            //// Act
            //var result = await controller.GetTreatmentLibraries();

            //// Assert
            //var okObjResult = result as OkObjectResult;
            //Assert.NotNull(okObjResult.Value);

            //var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType(okObjResult.Value,
            //    typeof(List<TreatmentLibraryDTO>));
            //Assert.Contains(dtos, t => t.Id == _testTreatmentLibrary.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            //// Arrange
            //Setup();
            //var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            //var controller = CreateAuthorizedController();
            //var budget = CreateScenarioTestData(simulation.Id);

            //// Act
            //var result = await controller.GetScenarioSelectedTreatments(simulation.Id);

            //// Assert
            //var okObjResult = result as OkObjectResult;
            //Assert.NotNull(okObjResult.Value);

            //var dtos = (List<TreatmentDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<TreatmentDTO>));
            //Assert.Single(dtos);

            //Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
            //Assert.Single(dtos[0].Consequences);
            //Assert.Single(dtos[0].Costs);
            //Assert.Single(dtos[0].BudgetIds);

            //Assert.Equal(_testScenarioTreatmentConsequence.Id, dtos[0].Consequences[0].Id);
            //Assert.Equal(_testScenarioTreatmentCost.Id, dtos[0].Costs[0].Id);
            //Assert.Contains(budget.Id, dtos[0].BudgetIds);
        }

        [Fact]
        public async Task ShouldModifyLibraryTreatmentData()
        {
            //// Arrange
            //Setup();
            //var controller = CreateAuthorizedControllerWithTreatmService();
            //CreateLibraryTestData();

            //var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();            
            //var dtoLibrary = dto.Where(t => t.Name == "Test Name").FirstOrDefault();
            //var treatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(dtoLibrary.Id);
            //dtoLibrary.Description = "Updated Description";
            //treatments[0].Name = "Updated Name";
            //treatments[0].CriterionLibrary = new CriterionLibraryDTO
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "",
            //    MergedCriteriaExpression = "",
            //    IsSingleUse = true
            //};
            //treatments[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "",
            //    MergedCriteriaExpression = "",
            //    IsSingleUse = true
            //};
            //treatments[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            //treatments[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "",
            //    MergedCriteriaExpression = "",
            //    IsSingleUse = true
            //};
            //treatments[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };

            //var sync = new PagingSyncModel<TreatmentDTO>()
            //{
            //    UpdateRows = new List<TreatmentDTO>() { treatments[0] },
            //    LibraryId = dtoLibrary.Id
            //};

            //var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            //{
            //    IsNewLibrary = false,
            //    Library = dtoLibrary,
            //    PagingSync = sync
            //};

            //// Act
            //await controller.UpsertTreatmentLibrary(libraryRequest);

            //// Assert
            //var modifiedDto =
            //    TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries().Single(lib => lib.Id == dtoLibrary.Id);
            //Assert.Equal(dtoLibrary.Description, modifiedDto.Description);
        }

        [Fact]
        public async Task ShouldModifyScenarioTreatmentData()
        {
            // Arrange
            Setup();
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedControllerWithTreatmService();
            CreateScenarioTestData(simulation.Id);

            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            dto[0].Description = "Updated Description";
            dto[0].Name = "Updated Name";
            dto[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].BudgetIds.Add(scenarioBudget.Id);

            var pageSync = new PagingSyncModel<TreatmentDTO>()
            {
                UpdateRows = new List<TreatmentDTO>() { dto[0] }
            };

            // Act
            await controller.UpsertScenarioSelectedTreatments(simulation.Id, pageSync);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            Assert.Equal(dto[0].Description, modifiedDto[0].Description);
            Assert.Equal(dto[0].Name, modifiedDto[0].Name);
            Assert.Equal(dto[0].BudgetIds.Count, modifiedDto[0].BudgetIds.Count);
            Assert.Contains(scenarioBudget.Id, modifiedDto[0].BudgetIds);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.DeleteTreatmentLibrary(_testTreatmentLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !TestHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
                    _.Id == _testTreatmentConsequence.Id));
        }
        [Fact]
        public async Task UserIsViewTreatmentFromLibraryAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTreatmentFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyTreatementFromScenarioAuthorized()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyTreatmentFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentModifyAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentModifyPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Editor));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyTreatmentFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteTreatmentFromLibraryAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentDeletePermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentDeleteAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.ReadOnly));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteTreatmentFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewTreatmentFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTreatmentFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
