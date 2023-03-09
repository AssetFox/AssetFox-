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
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using Microsoft.Data.SqlClient;
using AppliedResearchAssociates.iAM.TestHelpers;

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
            var nonexistentAttributeName = "nonexistentAttribute";
            var goal = DeficientConditionGoalDtos.Dto(goalId, nonexistentAttributeName);
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

        [Fact]
        public async Task UpsertScenarioDeficientConditionGoals_ThrowsDuringLaterDbCall_NothingChanges()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var goalId = Guid.NewGuid();
            var goalId2 = Guid.NewGuid();
            var attributeName = TestAttributeNames.CulvDurationN;
            var goal = DeficientConditionGoalDtos.Dto(goalId, attributeName);
            var goal2 = DeficientConditionGoalDtos.Dto(goalId2, attributeName);
            var goals = new List<DeficientConditionGoalDTO> { goal, goal2 };
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(
                goals, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var goalIndexToDelete = localScenarioDeficientGoals.FindIndex(g => g.Id == goalId2);
            var goalToUpdate = localScenarioDeficientGoals.First();
            var updatedGoalId = goalToUpdate.Id;
            var deleteGoalId = localScenarioDeficientGoals[1].Id;
            goalToUpdate.Name = "Updated";
            goalToUpdate.CriterionLibrary = criterionLibrary;
            goalToUpdate.DeficientLimit = double.NaN;
            var goalsBefore = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var controller = CreateController();
            var pagingSync = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                UpdateRows = new List<DeficientConditionGoalDTO> { goalToUpdate },
                RowsForDeletion = new List<Guid> { deleteGoalId },
            };

            // Act
            var exception = await Assert.ThrowsAsync<SqlException>(async () => await controller.UpsertScenarioDeficientConditionGoals(simulationId, pagingSync));

            // Assert
            var goalsAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            ObjectAssertions.Equivalent(goalsBefore, goalsAfter);
            Assert.Equal(2, goalsAfter.Count);
        }
    }
}
