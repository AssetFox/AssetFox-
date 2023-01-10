using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.BudgetPriority;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class BudgetPriorityTests
    {
        private ScenarioBudgetEntity _testScenarioBudget;
        private ScenarioBudgetPriorityEntity _testScenarioBudgetPriority;
        private BudgetPercentagePairEntity _testBudgetPercentagePair;
        private BudgetPriorityLibraryEntity _testBudgetPriorityLibrary;
        private BudgetPriorityEntity _testBudgetPriority;
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }

        private BudgetPriorityController CreateAuthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new BudgetPriorityController(
                EsecSecurityMocks.AdminMock.Object,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object,
                new BudgetPriorityPagingService(TestHelper.UnitOfWork));
            return controller;
        }
        private BudgetPriorityController CreateTestController(List<string> uClaims)
        {
            var testUser = ClaimsPrincipals.WithNameClaims(uClaims);
            var hubService = HubServiceMocks.Default();
            var accessor = HttpContextAccessorMocks.Default();
            var controller = new BudgetPriorityController(
                EsecSecurityMocks.AdminMock.Object,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object,
                new BudgetPriorityPagingService(TestHelper.UnitOfWork));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private void CreateLibraryTestData()
        {
            _testBudgetPriorityLibrary = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = BudgetPriorityLibraryDtos.BudgetPriorityLibraryEntityName };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPriorityLibrary);


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
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPriority);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "ScenarioBudgetEntity"
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


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
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudgetPriority);


            _testBudgetPercentagePair = new BudgetPercentagePairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioBudgetPriorityId = _testScenarioBudgetPriority.Id,
                ScenarioBudgetId = _testScenarioBudget.Id,
                Percentage = 100
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPercentagePair);
        }

        private BudgetPriorityController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new BudgetPriorityPagingService(unitOfWork.Object);
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var controller = new BudgetPriorityController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                service
                );
            return controller;
        }

        [Fact]
        public async Task GetBudgetPriorityLibraries_RepoReturns_UserIsAdmin_ReturnsLibraries()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);

            var result = await controller.GetBudgetPriorityLibraries();

            ActionResultAssertions.OkObject(result);
            var invocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.GetBudgetPriortyLibrariesNoChildren));
        }

        [Fact]
        public async Task GetSimulationBudgetPriorities_RepoReturns_UserIsAdmin_ReturnsLibraries()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();

            var result = await controller.GetScenarioBudgetPriorities(simulationId);

            ActionResultAssertions.OkObject(result);
            var invocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.GetScenarioBudgetPriorities));
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = new BudgetPriorityLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                BudgetPriorities = new List<BudgetPriorityDTO>()
            };

            var request = new LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO>()
            {
                Library = dto,
                IsNewLibrary = true
            };

            // Act
            var result = await controller
                .UpsertBudgetPriorityLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryInvocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.UpsertBudgetPriorityLibrary));
            var budgetPriorityInvocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.UpsertOrDeleteBudgetPriorities));
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).Returns(new List<BudgetPriorityDTO>());
            budgetRepo.Setup(br => br.GetScenarioSimpleBudgetDetails(simulationId)).Returns(new List<SimpleBudgetDetailDTO>());
            var controller = CreateController(unitOfWork);
            var dtos = new List<BudgetPriorityDTO>();
            var request = new PagingSyncModel<BudgetPriorityDTO>();

            // Act
            var result = await controller
                .UpsertScenarioBudgetPriorities(simulationId, request);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var result = await controller.DeleteBudgetPriorityLibrary(simulationId);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.DeleteBudgetPriorityLibrary));
            Assert.Equal(simulationId, invocation.Arguments.First());
        }

        [Fact]
        public async Task ShouldGetLibraryNoData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var dto = BudgetPriorityLibraryDtos.New();
            var dtos = new List<BudgetPriorityLibraryDTO> { dto };
            budgetPriorityRepo.Setup(b => b.GetBudgetPriortyLibrariesNoChildren()).Returns(dtos.ToList());
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.GetBudgetPriorityLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            var returnedDtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<BudgetPriorityLibraryDTO>));
            ObjectAssertions.Equivalent(dtos, returnedDtos);
        }

        [Fact]
        public async Task ShouldGetScenarioData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
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
        public async Task GetScenarioBudgetPriorityPageData()
        {
            // wjwjwj move to service level?
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);
            var request = new PagingRequestModel<BudgetPriorityDTO>();
            // Act
            var result = await controller.GetScenarioBudgetPriorityPage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<BudgetPriorityDTO>));
            var dtos = page.Items;
            Assert.Single(dtos);
            Assert.Equal(_testScenarioBudgetPriority.Id, dtos[0].Id);
            Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
            Assert.Equal(_testScenarioBudgetPriority.Year, dtos[0].Year);

            Assert.Single(dtos[0].BudgetPercentagePairs);
            Assert.Equal(_testBudgetPercentagePair.Id, dtos[0].BudgetPercentagePairs[0].Id);
            Assert.Equal(_testBudgetPercentagePair.Percentage, dtos[0].BudgetPercentagePairs[0].Percentage);
            Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dtos[0].BudgetPercentagePairs[0].BudgetId);
            Assert.Equal(_testScenarioBudget.Name, dtos[0].BudgetPercentagePairs[0].BudgetName);
        }

        [Fact]
        public async Task ShouldGetLibraryBudgetPriorityPageData()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();
            var dto = _testBudgetPriorityLibrary.ToDto();
            var request = new PagingRequestModel<BudgetPriorityDTO>();
            // Act
            var result = await controller.GetLibraryBudgetPriortyPage(dto.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<BudgetPriorityDTO>));
            var dtos = page.Items;
            Assert.Single(dtos);
            Assert.Equal(_testBudgetPriority.Id, dtos[0].Id);
            Assert.Equal(_testBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
            Assert.Equal(_testBudgetPriority.Year, dtos[0].Year);
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
            var updatedPriority = dto.BudgetPriorities[0];
            updatedPriority.PriorityLevel = 2;
            updatedPriority.Year = DateTime.Now.Year + 1;
            updatedPriority.CriterionLibrary = new CriterionLibraryDTO();

            var request = new LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO>()
            {
                Library = dto,
                PagingSync = new PagingSyncModel<BudgetPriorityDTO>()
                {
                    UpdateRows = new List<BudgetPriorityDTO>() { updatedPriority }
                }
            };

            // Act
            await controller.UpsertBudgetPriorityLibrary(request);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().Single(l => l.Id == dto.Id);
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
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Arrange
            _testScenarioBudgetPriority.BudgetPercentagePairs =
                new List<BudgetPercentagePairEntity> { _testBudgetPercentagePair };
            var dtos = new List<BudgetPriorityDTO> { _testScenarioBudgetPriority.ToDto() };
            var updatedPriorty = dtos[0];
            updatedPriorty.PriorityLevel = 2;
            updatedPriorty.Year = DateTime.Now.Year + 1;
            updatedPriorty.CriterionLibrary = new CriterionLibraryDTO();
            updatedPriorty.BudgetPercentagePairs[0].Percentage = 90;
            var request = new PagingSyncModel<BudgetPriorityDTO>()
            {
                UpdateRows = new List<BudgetPriorityDTO>() { updatedPriorty }
            };
            // Act
            await controller.UpsertScenarioBudgetPriorities(simulation.Id, request);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id)[0];
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
                !TestHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == _testBudgetPriorityLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == _testBudgetPriority.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                    _.BudgetPriorityId == _testBudgetPriority.Id));
        }

        [Fact]
        public async Task UserIsViewBudgetPriorityFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name, BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewBudgetPriorityFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);

        }
        [Fact]
        public async Task UserIsModifyBudgetPriorityFromScenarioAuthorized()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyBudgetPriorityFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyBudgetPriorityFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteBudgetPriorityFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteBudgetPriorityFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementModifyAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AttributesUpdateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteBudgetPriorityFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewBudgetPriorityFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name, BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewBudgetPriorityFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }

    }
}
