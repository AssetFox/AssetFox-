using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.BudgetPriority;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class BudgetPriorityControllerTests
    {
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

            budgetPriorityRepo.Setup(r => r.GetBudgetPriortyLibrariesNoChildren()).Returns(new List<BudgetPriorityLibraryDTO>());
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

            // Create controller manually to modify the service
            var service = BudgetPriorityPagingServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();

            var dto = new BudgetPriorityLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                BudgetPriorities = new List<BudgetPriorityDTO>()
            };
            var request = new LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO>()
            {
                Library = dto,
                IsNewLibrary = false
            };
            var user = UserDtos.Admin();
            var libraryUser = LibraryUserDtos.Modify(user.Id);
            service.SetupGetSyncedLibraryDataset(request);
            var controller = new BudgetPriorityController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                service.Object
                );
            
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(user.Id, libraryUser);
            budgetPriorityRepo.SetupGetLibraryAccess(request.Library.Id, libraryExists);
                        
            // Act
            var result = await controller
                .UpsertBudgetPriorityLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryInvocation = budgetPriorityRepo.SingleInvocationWithName(nameof(IBudgetPriorityRepository.UpsertOrDeleteBudgetPriorityLibraryAndPriorities));
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
            var response = new PagingPageModel<BudgetPriorityDTO>()
            {
                Items = dtos,
            };
            budgetPriorityRepo.Setup(br => br.GetScenarioBudgetPriorities(simulationId)).Returns(dtos);

            // Create controller manually to modify the service
            var service = BudgetPriorityPagingServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            service.Setup(s => s.GetScenarioPage(simulationId, request)).Returns(response);
            var controller = new BudgetPriorityController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                service.Object
                );

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
            var response = new PagingPageModel<BudgetPriorityDTO>()
            {
                Items = dtos,
            };

            // Create controller manually to modify the service
            var service = BudgetPriorityPagingServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            service.Setup(s => s.GetLibraryPage(libraryId, request)).Returns(response);
            var controller = new BudgetPriorityController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                service.Object
                );

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
    }
}
