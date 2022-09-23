﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class BudgetPriorityTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private ScenarioBudgetEntity _testScenarioBudget;
        private ScenarioBudgetPriorityEntity _testScenarioBudgetPriority;
        private BudgetPercentagePairEntity _testBudgetPercentagePair;
        private BudgetPriorityLibraryEntity _testBudgetPriorityLibrary;
        private BudgetPriorityEntity _testBudgetPriority;
        private const string BudgetPriorityLibraryEntityName = "BudgetPriorityLibraryEntity";
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            _testHelper.CreateSingletons();
        }

        private BudgetPriorityController CreateAuthorizedController()
        {
            var controller = new BudgetPriorityController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object,
                _mockClaimHelper.Object);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            return controller;
        }

        private BudgetPriorityController CreateUnauthorizedController()
        {
            var controller = new BudgetPriorityController(
                _testHelper.MockEsecSecurityDBE.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object,
                _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            return controller;
        }

        private void CreateLibraryTestData()
        {
            _testBudgetPriorityLibrary = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = BudgetPriorityLibraryEntityName };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPriorityLibrary);


            _testBudgetPriority = new BudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                BudgetPriorityLibraryId = _testBudgetPriorityLibrary.Id,
                PriorityLevel = 1,
                CriterionLibraryBudgetPriorityJoin = new CriterionLibraryBudgetPriorityEntity
                {
                    BudgetPriority = _testBudgetPriority,
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPriority);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "ScenarioBudgetEntity"
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testScenarioBudgetPriority = new ScenarioBudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                PriorityLevel = 1,
                CriterionLibraryScenarioBudgetPriorityJoin = new CriterionLibraryScenarioBudgetPriorityEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudgetPriority);


            _testBudgetPercentagePair = new BudgetPercentagePairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioBudgetPriorityId = _testScenarioBudgetPriority.Id,
                ScenarioBudgetId = _testScenarioBudget.Id,
                Percentage = 100
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPercentagePair);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();

            // Act
            var result = await controller.GetBudgetPriorityLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            // Arrange
            Setup();
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();

            // Act
            var result = await controller.GetScenarioBudgetPriorities(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            var dto = new BudgetPriorityLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                BudgetPriorities = new List<BudgetPriorityDTO>()
            };

            // Act
            var result = await controller
                .UpsertBudgetPriorityLibrary(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            var dtos = new List<BudgetPriorityDTO>();

            // Act
            var result = await controller
                .UpsertScenarioBudgetPriorities(simulation.Id, dtos);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            Setup();
            var controller = CreateAuthorizedController();
            var result = await controller.DeleteBudgetPriorityLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryData()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.GetBudgetPriorityLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<BudgetPriorityLibraryDTO>));
            Assert.Contains(dtos, b => b.Name == BudgetPriorityLibraryEntityName);
            var budgetPriorityLibraryDTO = dtos.FirstOrDefault(b => b.Name == BudgetPriorityLibraryEntityName && b.Id == _testBudgetPriorityLibrary.Id);
            Assert.True(dtos[0].BudgetPriorities.Count() > 0);
            Assert.Equal(_testBudgetPriority.PriorityLevel, budgetPriorityLibraryDTO.BudgetPriorities[0].PriorityLevel);
            Assert.Equal(_testBudgetPriority.Year, budgetPriorityLibraryDTO.BudgetPriorities[0].Year);
            Assert.Equal(_testBudgetPriority.CriterionLibraryBudgetPriorityJoin.CriterionLibraryId, budgetPriorityLibraryDTO.BudgetPriorities[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioData()
        {
            // Arrange
            Setup();
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Act
            var result = await controller.GetScenarioBudgetPriorities(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<BudgetPriorityDTO>));
            Assert.Single(dtos);
            Assert.Equal(_testScenarioBudgetPriority.Id, dtos[0].Id);
            Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
            Assert.Equal(_testScenarioBudgetPriority.Year, dtos[0].Year);

            Assert.Single(dtos[0].BudgetPercentagePairs);
            Assert.Equal(_testBudgetPercentagePair.Id, dtos[0].BudgetPercentagePairs[0].Id);
            Assert.Equal(_testBudgetPercentagePair.Percentage, dtos[0].BudgetPercentagePairs[0].Percentage);
            Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dtos[0].BudgetPercentagePairs[0].BudgetId);
            Assert.Equal(_testScenarioBudget.Name, dtos[0].BudgetPercentagePairs[0].BudgetName);

            Assert.Equal(_testScenarioBudgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId, dtos[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldModifyLibraryData()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Arrange
            _testBudgetPriorityLibrary.BudgetPriorities = new List<BudgetPriorityEntity> { _testBudgetPriority };

            var dto = _testBudgetPriorityLibrary.ToDto();
            dto.Description = "Updated Description";
            dto.BudgetPriorities[0].PriorityLevel = 2;
            dto.BudgetPriorities[0].Year = DateTime.Now.Year + 1;
            dto.BudgetPriorities[0].CriterionLibrary = new CriterionLibraryDTO();

            // Act
            await controller.UpsertBudgetPriorityLibrary(dto);

            // Assert
            var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().Single(l => l.Id == dto.Id);
            Assert.Equal(dto.Description, modifiedDto.Description);

            Assert.Equal(dto.BudgetPriorities[0].PriorityLevel, modifiedDto.BudgetPriorities[0].PriorityLevel);
            Assert.Equal(dto.BudgetPriorities[0].Year, modifiedDto.BudgetPriorities[0].Year);
            Assert.Equal(dto.BudgetPriorities[0].CriterionLibrary.Id,
                modifiedDto.BudgetPriorities[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioData()
        {
            // Arrange
            Setup();
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Arrange
            _testScenarioBudgetPriority.BudgetPercentagePairs =
                new List<BudgetPercentagePairEntity> { _testBudgetPercentagePair };
            var dtos = new List<BudgetPriorityDTO> { _testScenarioBudgetPriority.ToDto() };

            dtos[0].PriorityLevel = 2;
            dtos[0].Year = DateTime.Now.Year + 1;
            dtos[0].CriterionLibrary = new CriterionLibraryDTO();
            dtos[0].BudgetPercentagePairs[0].Percentage = 90;

            // Act
            await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);

            // Assert
            var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id)[0];
            Assert.Equal(dtos[0].PriorityLevel, modifiedDto.PriorityLevel);
            Assert.Equal(dtos[0].Year, modifiedDto.Year);
            Assert.Equal(dtos[0].CriterionLibrary.Id, modifiedDto.CriterionLibrary.Id);
            Assert.Equal(dtos[0].BudgetPercentagePairs[0].Percentage, modifiedDto.BudgetPercentagePairs[0].Percentage);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.DeleteBudgetPriorityLibrary(_testBudgetPriorityLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !_testHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == _testBudgetPriorityLibrary.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == _testBudgetPriority.Id));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                    _.BudgetPriorityId == _testBudgetPriority.Id));
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnInvestmentPost()
        {
            // Arrange
            Setup();
            var simulation = _testHelper.CreateSimulation();
            var controller = CreateUnauthorizedController();
            CreateScenarioTestData(simulation.Id);

            var dtos = new List<BudgetPriorityDTO>();

            // Act
            var result = await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        [Fact]
        public async Task UserIsViewBudgetPriorityFromLibraryAuthorized()
        {

            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();
            var url = "api/BudgetPriority/GetBudgetPriorityLibraries";
            var expected = HttpStatusCode.Unauthorized;

            var response = await client.GetAsync(url);
            Assert.Equal(expected, response.StatusCode);


            // Set up the claims for testing
            List<string> testClaims = new List<string>()
            {
                BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewAnyFromLibraryAccess,
                BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess
            };
            // Admin not authorized
            var controller = CreateUnauthorizedController();
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Administrator", new List<string>());
            // Assert
            var result = await controller.GetBudgetPriorityLibraries();
            Assert.IsType<UnauthorizedResult>(result);

            // Admin authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Administrator", testClaims);
            controller = CreateAuthorizedController();
            result = await controller.GetBudgetPriorityLibraries();
            Assert.IsType<OkObjectResult>(result);

            // Non-admin authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Editor", testClaims);
            controller = CreateAuthorizedController();
            result = await controller.GetBudgetPriorityLibraries();
            Assert.IsType<OkObjectResult>(result);

            // Non-admin not authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Editor", new List<string>());
            controller = CreateAuthorizedController();
            result = await controller.GetBudgetPriorityLibraries();
            Assert.IsType<UnauthorizedResult>(result);

            // Non-admin B2C not authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, "Editor", new List<string>());
            controller = CreateAuthorizedController();
            result = await controller.GetBudgetPriorityLibraries();
            Assert.IsType<UnauthorizedResult>(result);
        }
        [Fact]
        public async Task UserIsModifyBudgetPriorityFromScenarioAuthorized()
        {
            var simulation = _testHelper.CreateSimulation();
            var dtos = new List<BudgetPriorityDTO>();

            // Set up the claims for testing
            List<string> testClaims = new List<string>()
            {
                BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyAnyFromScenarioAccess,
                BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyPermittedFromScenarioAccess
            };
            // Admin not authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Administrator", new List<string>());
            var controller = CreateAuthorizedController();
            var result = await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);
            Assert.IsType<UnauthorizedResult>(result);
            // Admin authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Administrator", testClaims);
            controller = CreateAuthorizedController();
            result = await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);
            Assert.IsType<OkObjectResult>(result);
            // Non-admin unauthorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Editor", new List<string>());
            controller = CreateAuthorizedController();
            result = await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);
            Assert.IsType<UnauthorizedResult>(result);
            // Non-Admin authorized
            _testHelper.SetAuthorizationClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, "Editor", testClaims);
            controller = CreateAuthorizedController();
            result = await controller.UpsertScenarioBudgetPriorities(simulation.Id, dtos);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
