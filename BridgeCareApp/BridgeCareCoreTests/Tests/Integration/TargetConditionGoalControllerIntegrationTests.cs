using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;

namespace BridgeCareCoreTests.Tests.Integration
{
    internal class TargetConditionGoalControllerIntegrationTests
    {
        private TargetConditionGoalController CreateController()
        {
            var attributeService = new AttributeService(TestHelper.UnitOfWork);
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            var targetConditionGoalService = new TargetConditionGoalPagingService(TestHelper.UnitOfWork);
            var controller = new TargetConditionGoalController(
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                claimHelper.Object,
                targetConditionGoalService);
            return controller;
        }

        public async Task CreateNewLibrary_Does()
        {
            var controller = CreateController();
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("Library");
            var library = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var goalId = Guid.NewGuid();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var goalName = RandomStrings.WithPrefix("Goal");
            var attributeName = TestAttributeNames.DeckDurationN;
            var goal = TargetConditionGoalDtos.Dto(attributeName, goalId, goalName);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
            {
                AddedRows = new List<TargetConditionGoalDTO> { goal }
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            await controller.UpsertTargetConditionGoalLibrary(upsertRequest);

            library.TargetConditionGoals = new List<TargetConditionGoalDTO> { goal };
            var librariesAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
            var libraryAfter = librariesAfter.Single(l => l.Id == libraryId);
            ObjectAssertions.EquivalentExcluding(library, libraryAfter, x => x.TargetConditionGoals[0].CriterionLibrary, x => x.Owner);
        }
    }
}
