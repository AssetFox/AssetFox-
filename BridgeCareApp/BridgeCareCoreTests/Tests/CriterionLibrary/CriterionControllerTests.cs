using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CriterionLibraryControllerTests
    {

        private CriterionLibraryController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CashFlowPagingService(unitOfWork.Object);
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var controller = new CriterionLibraryController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object
                );
            return controller;
        }

        [Fact]
        public async Task CriterionLibraries_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.CriterionLibraries();

            // Assert
            ActionResultAssertions.OkObject(result);
            criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.CriterionLibraries));
        }

        [Fact]
        public async Task UpsertCriterionLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var repo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = CriterionLibraryTestSetup.TestCriterionLibrary();

            // Act
            var result = await controller
                .UpsertCriterionLibrary(dto);

            // Assert
            ActionResultAssertions.OkObject(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.UpsertCriterionLibrary));
            Assert.Equal(dto, invocation.Arguments[0]);
        }

        [Fact]
        public async Task DeleteCriterionLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var repo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            // Act
            var result = await controller.DeleteCriterionLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.DeleteCriterionLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
        }
    }
}
