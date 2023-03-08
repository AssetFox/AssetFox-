using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using BridgeCareCore.Controllers;
using Microsoft.SqlServer.Dac.Model;
using BridgeCareCore.Services;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class DeficientConditionGoalControllerIntegrationTests
    {
        private DeficientConditionGoalController CreateController()
        {
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            var service = new DeficientConditionGoalPagingService(TestHelper.UnitOfWork);
            var controller = new DeficientConditionGoalController(
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                claimHelper.Object,
                service);
            return controller;
        }

        [Fact]
        public async Task UpsertDeficientConditionGoalLibraryAndGoals_SecondPartFails_NothingHappens()
        {
            var library = DeficientConditionGoalLibraryTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork);
            var goalId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var attributeName = "nonexistentAttribute";
            var goal = DeficientConditionGoalDtos.Dto(goalId, criterionLibraryId, attributeName);
            library.DeficientConditionGoals.Add(goal);
            library.Description = "Updated description";
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                AddedRows = new List<DeficientConditionGoalDTO> { goal },
            };

            var upsertRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                Library = library,
                IsNewLibrary = false,
                SyncModel = syncModel,
            };

            var controller = CreateController();
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await controller.UpsertDeficientConditionGoalLibrary(
                upsertRequest));

            var libraryAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()
                .Single(lib => lib.Id == library.Id);
            Assert.Null(libraryAfter.Description);
        }
    }
}
