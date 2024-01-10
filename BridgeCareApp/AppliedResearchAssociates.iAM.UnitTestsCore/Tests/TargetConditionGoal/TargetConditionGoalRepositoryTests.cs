using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.TestHelpers.Assertions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TargetConditionGoal;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Extensions.DependencyModel;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class TargetConditionGoalRepositoryTests
    {

        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");

        private void SetupAttributesAndNetwork()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        private TargetConditionGoalLibraryDTO SetupLibraryForGet()
        {
            var dto = TargetConditionGoalLibraryDtos.Dto();
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
            return dto;
        }


        private CriterionLibraryDTO SetupCriterionLibraryForUpsertOrDelete()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            return criterionLibrary;
        }

        private TargetConditionGoalDTO SetupForScenarioTargetGet(Guid simulationId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal = TargetConditionGoalDtos.Dto(attribute.Name);
            var goals = new List<TargetConditionGoalDTO> { goal };
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(goals, simulationId);
            return goal;
        }

        private CriterionLibraryDTO SetupForScenarioTargetUpsertOrDelete(Guid simulationId)
        {
            SetupForScenarioTargetGet(simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            return criterionLibrary;
        }

        private void JoinCriterionToLibraryConditionGoal(Guid goalId, Guid criterionId)
        {
            var libraryJoin = new CriterionLibraryTargetConditionGoalEntity()
            {
                CriterionLibraryId = criterionId,
                TargetConditionGoalId = goalId
            };

            TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Add(libraryJoin);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public void UpsertTargetConditionGoalLibary_DoesNotThrow()
        {
            SetupAttributesAndNetwork();
            var library = SetupLibraryForGet();
            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo
                .UpsertTargetConditionGoalLibrary(library);
        }

        [Fact]
        public void DeleteTargetConditionGoalLibrary_LibraryDoesNotExist_DoesNotThrow()
        {
            SetupAttributesAndNetwork();
            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(Guid.NewGuid()); ;
        }

        [Fact]
        public void GetTargetConditionGoalLibrariesNoChildren_LibraryInDb_Gets()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var library = SetupLibraryForGet();
            var attribute = AttributeDtos.CulvDurationN;
            var goal = TargetConditionGoalTestSetup.ModelForLibraryGoalInDb(library.Id, attribute.Name);

            // Act
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildren();
            var dto = dtos.Single(d => d.Id == library.Id);
        }

        [Fact]
        public void UpsertOrDeleteTargetConditionGoals_GoalInDbWithLibrary_ModifiesGoal()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var libraryDto = SetupLibraryForGet();
            var attribute = AttributeDtos.CulvDurationN;
            var goalDto = TargetConditionGoalTestSetup.ModelForLibraryGoalInDb(libraryDto.Id, attribute.Name);
            goalDto.Name = "Updated Name";
            goalDto.CriterionLibrary = criterionLibrary;
            var updateRows = new List<TargetConditionGoalDTO> { goalDto };

            // Act //UpsertOrDeleteTargetConditionGoals
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(updateRows, libraryDto.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.TargetConditionGoalRepo
                .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                .Single(x => x.Id == libraryDto.Id);

            Assert.Equal("Updated Name", modifiedDto.TargetConditionGoals[0].Name);
            Assert.Equal(goalDto.Attribute, modifiedDto.TargetConditionGoals[0].Attribute);
        }

        [Fact]
        public void UpsertTargetConditionGoalLibrary_LibraryInDb_Modifies()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var library = SetupLibraryForGet();
            library.Description = "Updated Description";

            // Act //UpsertOrDeleteTargetConditionGoals
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(library);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.TargetConditionGoalRepo
                .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                .Single(x => x.Id == library.Id);
            Assert.Equal(library.Description, modifiedDto.Description);
        }

        [Fact]
        public async Task UpsertTargetConditionGoalLibraryAndGoals_LibraryNotInDb_Adds()
        {
            SetupAttributesAndNetwork();
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("Library");
            var library = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var goalId = Guid.NewGuid();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var goalName = RandomStrings.WithPrefix("Goal");
            var attributeName = TestAttributeNames.DeckDurationN;
            var goal = TargetConditionGoalDtos.Dto(attributeName, goalId, goalName);
            library.TargetConditionGoals = new List<TargetConditionGoalDTO> { goal };

            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibraryGoalsAndPossiblyUser(library, true, user.Id);

            var librariesAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
            var libraryAfter = librariesAfter.Single(l => l.Id == libraryId);
            ObjectAssertions.EquivalentExcluding(library, libraryAfter, x => x.TargetConditionGoals[0].CriterionLibrary, x => x.Owner);
        }

        [Fact]
        public void UpsertTargetConditionGoalLibraryAndGoals_LibraryInDb_Updates()
        {
            SetupAttributesAndNetwork();
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("Library");
            var library = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var goalId = Guid.NewGuid();
            var goalName = RandomStrings.WithPrefix("Goal");
            var attributeName = TestAttributeNames.DeckDurationN;
            var goal = TargetConditionGoalDtos.Dto(attributeName, goalId, goalName);
            library.TargetConditionGoals = new List<TargetConditionGoalDTO> { goal };
            library.Description = "Updated library description";

            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibraryGoalsAndPossiblyUser(library, false, Guid.NewGuid());

            var librariesAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
            var libraryAfter = librariesAfter.Single(l => l.Id == libraryId);
            ObjectAssertions.EquivalentExcluding(library, libraryAfter, x => x.TargetConditionGoals[0].CriterionLibrary, x => x.Owner);
        }

        [Fact]
        public void DeleteTargetConditionGoalLibrary_DeletesGoalsAndData()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var targetConditionGoalLibraryEntity = SetupLibraryForGet();
            var libraryId = targetConditionGoalLibraryEntity.Id;
            var attribute = TestAttributeNames.CulvDurationN;
            var targetConditionGoalEntity = TargetConditionGoalTestSetup.ModelForLibraryGoalInDb(libraryId, attribute);
            var goalId = targetConditionGoalEntity.Id;

            JoinCriterionToLibraryConditionGoal(goalId, criterionLibrary.Id);

            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);

            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == goalId));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Any(_ =>
                    _.TargetConditionGoalId == TargetConditionGoalId));
        }

        [Fact]
        public void GetScenarioTargetConditionGoals_ScenarioInDbWithGoal_Gets()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var goal = SetupForScenarioTargetGet(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public void ShouldGetLibraryTargetConditionGoalPageData()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = TargetConditionGoalTestSetup.ModelForLibraryGoalInDb(library.Id, TestAttributeNames.CulvDurationN);

            // Act
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(library.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public void UpsertOrDeleteScenarioTargetConditionGoals_ScenarioInDbWithGoals_UpdatesOneDeletesAnother()
        {
            SetupAttributesAndNetwork();
            // Arrange
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var criterionLibrary = SetupForScenarioTargetUpsertOrDelete(simulation.Id);
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

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
            var goals = new List<TargetConditionGoalDTO> { addedGoal, goalToUpdate };

            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(goals, simulation.Id);

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
        [Fact]
        public async Task UpdateTargetConditionGoalLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);
            TargetConditionGoalLibraryUserTestSetup.SetUsersOfTargetConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }
        [Fact]
        public async Task UpdateTargetConditionGoalLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);
            TargetConditionGoalLibraryUserTestSetup.SetUsersOfTargetConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }
        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);
            TargetConditionGoalLibraryUserTestSetup.SetUsersOfTargetConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }

        [Fact]
        public void GetLibraryModifiedDate_Does()
        {
            var before = DateTime.Now;
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);
            var after = DateTime.Now;

            var actual = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryModifiedDate(library.Id);

            DateTimeAssertions.Between(before, after, actual, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task GetTargetConditionGoalLibrariesNoChildrenAccessibleToUser_InaccessibleLibraryInDb_DoesNotGet()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);

            var libraries = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildrenAccessibleToUser(user.Id);

            Assert.DoesNotContain(libraries, l => l.Id == library.Id);
        }

        [Fact]
        public async Task GetLibraryAccess_UserHasLibraryAccess_Gets()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);
            TargetConditionGoalLibraryUserTestSetup.SetUsersOfTargetConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var access = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryAccess(library.Id, user.Id);

            var returnedAccess = access.Access;
            var expected = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = user.Id,
                UserName = user.Username,
            };
            ObjectAssertions.Equivalent(expected, returnedAccess);
        }


        [Fact]
        public async Task GetLibraryAccess_UserDoesNotHaveAccess_Gets()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TargetConditionGoalLibraryTestSetup.ModelForEntityInDbWithSingleGoal(TestHelper.UnitOfWork);

            var access = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetLibraryAccess(library.Id, user.Id);

            var expected = new LibraryUserAccessModel
            {
                Access = null,
                UserId = user.Id,
                LibraryExists = true,
            };
            ObjectAssertions.Equivalent(expected, access);
        }
    }
}
