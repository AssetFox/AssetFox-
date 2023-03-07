using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Tests.BudgetPriority;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class BudgetPriorityControllerIntegrationTests
    {
        public BudgetPriorityController CreateController()
        {
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            var service = new BudgetPriorityPagingService(TestHelper.UnitOfWork);
            var controller = new BudgetPriorityController(
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                claimHelper.Object,
                service
               );
            return controller;
        }

        [Fact]
        public async Task UpsertBudgetPriorityLibrary_ChildUpdateFails_NoChanges()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = BudgetPriorityLibraryDtos.New(libraryId);
            var library2 = BudgetPriorityLibraryDtos.New();
            library2.Name = "Updated name";
            var priorityId = Guid.NewGuid();
            var childDto = BudgetPriorityDtos.New(priorityId);
            var childDto2 = BudgetPriorityDtos.New(priorityId);
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.WithPrefix("Budget");
            var criterionLibrary = CriterionLibraryDtos.Dto();
            criterionLibrary.Name = null;
            childDto.CriterionLibrary = criterionLibrary;
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(library);
            var budgetPriorities = new List<BudgetPriorityDTO> { childDto, childDto2 };

            var controller = CreateController();
            var upsertRequest = new LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO>();
            upsertRequest.Library = library;
            var syncModel = new PagingSyncModel<BudgetPriorityDTO> { AddedRows = budgetPriorities };
            upsertRequest.SyncModel = syncModel;

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
            await controller.UpsertBudgetPriorityLibrary(upsertRequest));

            var librariesAfter = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();
            var libraryAfter = librariesAfter.Single(
                lib => lib.Id == libraryId);
            Assert.Equal(library.Name, libraryAfter.Name);
        }
    }
}
