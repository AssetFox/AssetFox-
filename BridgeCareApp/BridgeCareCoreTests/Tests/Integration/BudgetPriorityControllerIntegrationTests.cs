using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
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
        public void UpsertBudgetPriorityLibrary_ChildUpdateFails_NoChanges()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = BudgetPriorityLibraryDtos.New(libraryId);
            var library2 = BudgetPriorityLibraryDtos.New();
            var childDto = BudgetPriorityDtos.New();
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.WithPrefix("Budget");
            var pairId = Guid.NewGuid();
            var percentagePair = new BudgetPercentagePairDTO
            {
                BudgetId = budgetId,
                BudgetName = budgetName,
                Id = pairId,
                Percentage = 2,
            };
            childDto.Year = int.MaxValue;
            childDto.BudgetPercentagePairs.Add(percentagePair);
            var criterionLibrary = CriterionLibraryDtos.Dto();
            criterionLibrary.Name = null;
            childDto.CriterionLibrary = criterionLibrary;
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(library);
            var budgetPriorities = new List<BudgetPriorityDTO> { childDto };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(
                budgetPriorities, libraryId);

            var controller = CreateController();
            
        }
    }
}
