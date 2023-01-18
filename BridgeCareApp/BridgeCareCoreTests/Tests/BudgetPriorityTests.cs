using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
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
    public class BudgetPriorityControllerTests
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
                new BudgetPriortyPagingService(TestHelper.UnitOfWork));
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
                new BudgetPriortyPagingService(TestHelper.UnitOfWork));
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
            var service = new BudgetPriortyPagingService(unitOfWork.Object);
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
        public async Task ShouldReturnOkResultOnLibraryUpdate()
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
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var dto = BudgetPriorityDtos.New();
            var dtos = new List<BudgetPriorityDTO> { dto };
            var simulationId = Guid.NewGuid();
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).Returns(dtos);
            var controller = CreateController(unitOfWork);
            
            // Act
            var result = await controller.GetScenarioBudgetPriorities(simulationId);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var returnDtos = (List<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<BudgetPriorityDTO>));
            var returnDto = returnDtos.Single();
            Assert.Equal(dto, returnDto);
        }

        [Fact]
        public async Task GetScenarioBudgetPriorityPageData()
        {
            // move to paging service level?
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var dto = BudgetPriorityDtos.New();
            var dtos = new List<BudgetPriorityDTO> { dto };
            var request = new PagingRequestModel<BudgetPriorityDTO>();
            budgetPriorityRepo.Setup(br => br.GetScenarioBudgetPriorities(simulationId)).Returns(dtos);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.GetScenarioBudgetPriorityPage(simulationId, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<BudgetPriorityDTO>));
            var returnDtos = page.Items;
            ObjectAssertions.Equivalent(dto, returnDtos.Single());
        }

        [Fact]
        public async Task ShouldGetLibraryBudgetPriorityPageData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var userRepository = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = BudgetPriorityDtos.New();
            var dtos = new List<BudgetPriorityDTO> { dto };
            budgetPriorityRepo.Setup(b => b.GetBudgetPrioritiesByLibraryId(libraryId)).Returns(dtos);
            var request = new PagingRequestModel<BudgetPriorityDTO>();
            var controller = CreateController(unitOfWork);
            
            // Act
            var result = await controller.GetLibraryBudgetPriortyPage(libraryId, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<BudgetPriorityDTO>));
            var returnDtos = page.Items;
            Assert.Equal(returnDtos.Single(), dto);
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
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var controller = CreateTestController(claims);
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
