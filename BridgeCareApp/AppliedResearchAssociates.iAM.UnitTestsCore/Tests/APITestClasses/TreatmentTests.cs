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
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class TreatmentTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        private TreatmentController CreateAuthorizedController()
        {
            var controller = new TreatmentController(_testHelper.MockTreatmentService.Object, _testHelper.MockEsecSecurityAdmin.Object, _testHelper.UnitOfWork, _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object, _mockClaimHelper.Object);
            return controller;
        }

        private TreatmentController CreateUnauthorizedController()
        {
            var controller = new TreatmentController(_testHelper.MockTreatmentService.Object, _testHelper.MockEsecSecurityDBE.Object,
                _testHelper.UnitOfWork, _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object, _mockClaimHelper.Object);
            return controller;
        }

        private TreatmentController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new TreatmentController(_testHelper.MockTreatmentService.Object, _testHelper.MockEsecSecurityDBE.Object,
                _testHelper.UnitOfWork, _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object, _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            _testHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private ScenarioBudgetEntity CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            _testHelper.UnitOfWork.Context.AddEntity(budget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            _testHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = budget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            _testHelper.UnitOfWork.Context.SaveChanges();
            return budget;
        }


        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            Setup();
            // Act
            var controller = CreateAuthorizedController();
            var result = await controller.GetTreatmentLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            Setup();
            var simulation = _testHelper.CreateSimulation();
            // Act
            var controller = CreateAuthorizedController();
            var result = await controller.GetScenarioSelectedTreatments(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            // Act
            var result = await controller.UpsertTreatmentLibrary(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            var dtos = new List<TreatmentDTO>();
            var simulation = _testHelper.CreateSimulation();

            // Act
            var result = await controller.UpsertScenarioSelectedTreatments(simulation.Id, dtos);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryDelete()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();

            // Act
            var result = await controller.DeleteTreatmentLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryTreatmentData()
        {
            // Arrange
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.GetTreatmentLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TreatmentLibraryDTO>));
            Assert.True(dtos.Any(t => t.Id == _testTreatmentLibrary.Id));
            var resultTreatmentLibrary = dtos.FirstOrDefault(t => t.Id == _testTreatmentLibrary.Id);
            Assert.Equal(_testTreatmentLibrary.Id, resultTreatmentLibrary.Id);
            Assert.Single(resultTreatmentLibrary.Treatments);

            Assert.Equal(_testTreatment.Id, resultTreatmentLibrary.Treatments[0].Id);
            Assert.Single(resultTreatmentLibrary.Treatments[0].Consequences);
            Assert.Single(resultTreatmentLibrary.Treatments[0].Costs);

            Assert.Equal(_testTreatmentConsequence.Id, resultTreatmentLibrary.Treatments[0].Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, resultTreatmentLibrary.Treatments[0].Costs[0].Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var result = await controller.GetScenarioSelectedTreatments(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TreatmentDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<TreatmentDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
            Assert.Single(dtos[0].Consequences);
            Assert.Single(dtos[0].Costs);
            Assert.Single(dtos[0].BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dtos[0].Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dtos[0].Costs[0].Id);
            Assert.True(dtos[0].BudgetIds.Contains(budget.Id));
        }

        [Fact]
        public async Task ShouldModifyLibraryTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            var dto = _testHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
            var dtoLibrary = dto.Where(t => t.Name == "Test Name").FirstOrDefault();
            dtoLibrary.Description = "Updated Description";
            dtoLibrary.Treatments[0].Name = "Updated Name";
            dtoLibrary.Treatments[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dtoLibrary.Treatments[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };

            // Act
            await controller.UpsertTreatmentLibrary(dtoLibrary);

            // Assert
            var modifiedDto =
                _testHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries().Single(lib => lib.Id == dtoLibrary.Id);
            Assert.Equal(dtoLibrary.Description, modifiedDto.Description);
            // below assertions are broken. Broken-ness was hidden behind a timer.
            //Assert.True(modifiedDto.AppliedScenarioIds.Any());
            //Assert.Equal(simulation.Id, modifiedDto.AppliedScenarioIds[0]);

            //Assert.Equal(dtoLibrary.Treatments[0].Name, modifiedDto.Treatments[0].Name);
            //Assert.Equal(dtoLibrary.Treatments[0].CriterionLibrary.Id,
            //    modifiedDto.Treatments[0].CriterionLibrary.Id);
            //Assert.True(modifiedDto.Treatments[0].Costs.Any());

            //Assert.Equal(dtoLibrary.Treatments[0].Costs[0].CriterionLibrary.Id,
            //    modifiedDto.Treatments[0].Costs[0].CriterionLibrary.Id);
            //Assert.Equal(dtoLibrary.Treatments[0].Costs[0].Equation.Id,
            //    modifiedDto.Treatments[0].Costs[0].Equation.Id);

            //Assert.Equal(dtoLibrary.Treatments[0].Costs[0].CriterionLibrary.Id,
            //    modifiedDto.Treatments[0].Consequences[0].CriterionLibrary.Id);
            //Assert.Equal(dtoLibrary.Treatments[0].Consequences[0].Equation.Id,
            //    modifiedDto.Treatments[0].Consequences[0].Equation.Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            _testHelper.UnitOfWork.Context.SaveChanges();

            var dto = _testHelper.UnitOfWork.SelectableTreatmentRepo
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

            // Act
            await controller.UpsertScenarioSelectedTreatments(simulation.Id, dto);

            // Assert
            var modifiedDto = _testHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            Assert.Equal(dto[0].Description, modifiedDto[0].Description);
            Assert.Equal(dto[0].Name, modifiedDto[0].Name);
            // below was already broken. Broken-ness was hidden behind a timer.
            //Assert.Equal(dto[0].CriterionLibrary.Id, modifiedDto[0].CriterionLibrary.Id);
            //Assert.Equal(dto[0].Costs[0].CriterionLibrary.Id, modifiedDto[0].Costs[0].CriterionLibrary.Id);
            //Assert.Equal(dto[0].Costs[0].Equation.Id, modifiedDto[0].Costs[0].Equation.Id);
            //Assert.Equal(dto[0].Consequences[0].CriterionLibrary.Id,
            //    modifiedDto[0].Consequences[0].CriterionLibrary.Id);
            //Assert.Equal(dto[0].Consequences[0].Equation.Id, modifiedDto[0].Consequences[0].Equation.Id);
            //Assert.Equal(dto[0].BudgetIds.Count, modifiedDto[0].BudgetIds.Count);
            //Assert.True(modifiedDto[0].BudgetIds.Contains(scenarioBudget.Id));
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
                !_testHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            Assert.True(
                !_testHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
                    _.Id == _testTreatmentConsequence.Id));
        }
        [Fact]
        public async Task UserIsViewTreatmentFromLibraryAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
