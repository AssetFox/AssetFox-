using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class TargetConditionGoalControllerIntegrationTests
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

        [Fact]
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

        [Fact]
        public async Task UpsertScenarioTargetConditionGoals_IdkWhat()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var controller = CreateController();
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal1 = TargetConditionGoalDtos.Dto(attribute.Name);
            var goals1 = new List<TargetConditionGoalDTO> { goal1 };
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(goals1, simulation.Id);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);

            var deletedTargetConditionId = Guid.NewGuid();
            TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(new ScenarioTargetConditionGoalEntity
            {
                Id = deletedTargetConditionId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Name = "Deleted"
            });
            TestHelper.UnitOfWork.Context.SaveChanges();

            var localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
            var indexToDelete = localScenarioTargetGoals.FindIndex(g => g.Id == deletedTargetConditionId);
            var deleteId = localScenarioTargetGoals[indexToDelete].Id;
            var goalToUpdate = localScenarioTargetGoals.Single(g => g.Id != deletedTargetConditionId);
            var updatedGoalId = goalToUpdate.Id;
            goalToUpdate.Name = "Updated";
            goalToUpdate.CriterionLibrary = criterionLibrary;
            var newGoalId = Guid.NewGuid();
            var addedGoal = new TargetConditionGoalDTO
            {
                Id = newGoalId,
                Attribute = attribute.Name,
                Name = "New"
            };
            var addedGoals = new List<TargetConditionGoalDTO> { addedGoal };
            var goalsToUpdate = new List<TargetConditionGoalDTO> { goalToUpdate };
            var request = new PagingSyncModel<TargetConditionGoalDTO>
            {
                AddedRows = addedGoals,
                UpdateRows = goalsToUpdate,
                RowsForDeletion = new List<Guid> { deletedTargetConditionId },
            };

            // Act
            await controller.UpsertScenarioTargetConditionGoals(simulation.Id, request);

            // Assert
            var serverScenarioTargetConditionGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo
                .GetScenarioTargetConditionGoals(simulation.Id);
            Assert.Equal(serverScenarioTargetConditionGoals.Count, serverScenarioTargetConditionGoals.Count);
            Assert.False(
                TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Any(_ => _.Id == deletedTargetConditionId));
            localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
            var localNewTargetGoal = localScenarioTargetGoals.Single(_ => _.Name == "New");
            var serverNewTargetGoal = localScenarioTargetGoals.FirstOrDefault(_ => _.Id == newGoalId);
            Assert.NotNull(serverNewTargetGoal);
            Assert.Equal(localNewTargetGoal.Attribute, serverNewTargetGoal.Attribute);

            var localUpdatedTargetGoal = localScenarioTargetGoals.Single(_ => _.Id == updatedGoalId);
            var serverUpdatedTargetGoal = serverScenarioTargetConditionGoals
                .FirstOrDefault(_ => _.Id == updatedGoalId);
            ObjectAssertions.Equivalent(localNewTargetGoal, serverNewTargetGoal);
            Assert.Equal(localUpdatedTargetGoal.Name, serverUpdatedTargetGoal.Name);
            Assert.Equal(localUpdatedTargetGoal.Attribute, serverUpdatedTargetGoal.Attribute);
            Assert.Equal(localUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression);
        }
    }
}
