using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DeficientConditionGoalRepositoryTests
    {
        private static void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }

        public DeficientConditionGoalLibraryEntity TestDeficientConditionGoalLibrary(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var name = RandomStrings.WithPrefix("Test Name");
            var entity = new DeficientConditionGoalLibraryEntity
            {
                Id = resolveId,
                Name = name,
            };
            return entity;
        }

        public DeficientConditionGoalEntity TestDeficientConditionGoal(Guid libraryId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var entity = new DeficientConditionGoalEntity
            {
                Id = resolveId,
                DeficientConditionGoalLibraryId = libraryId,
                Name = "Test Name",
                AllowedDeficientPercentage = 100,
                DeficientLimit = 1.0
            };
            return entity;
        }

        private DeficientConditionGoalEntity SetupLibraryForGet(Guid libraryId, Guid goalId)
        {
            var library = TestDeficientConditionGoalLibrary(libraryId);
            TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Add(library);
            var goal = TestDeficientConditionGoal(libraryId, goalId);
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            goal.AttributeId = attribute.Id;
            TestHelper.UnitOfWork.Context.DeficientConditionGoal.Add(goal);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return goal;
        }

        public ScenarioDeficientConditionGoalEntity TestScenarioDeficientConditionGoal(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var entity = new ScenarioDeficientConditionGoalEntity
            {
                Id = resolveId,
                Name = "Test Name",
                AllowedDeficientPercentage = 100,
                DeficientLimit = 1.0
            };
            return entity;
        }

        private void SetupScenarioGoalsForGet(Guid simulationId, Guid goalId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal = TestScenarioDeficientConditionGoal(goalId);
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            goal.AttributeId = attribute.Id;
            goal.SimulationId = simulationId;
            goal.Attribute = attribute;
            TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Add(goal);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            Setup();

            // Act and assert
            var result = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren();
        }

        [Fact]
        public void UpsertDeficientConditionGoalLibrary_Does()
        {
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            Setup();
            SetupLibraryForGet(libraryId, goalId);
            var library = TestDeficientConditionGoalLibrary(libraryId).ToDto();

            // Act
            TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .UpsertDeficientConditionGoalLibrary(library);

            // Assert
            var libraryAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren()
                .Single(l => l.Id == libraryId);
            ObjectAssertions.Equivalent(library, libraryAfter);
        }

        [Fact]
        public void UpsertDeficientConditionGoals_LibraryInDb_Does()
        {
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            Setup();
            SetupLibraryForGet(libraryId, goalId);
            var library = TestDeficientConditionGoalLibrary(libraryId).ToDto();
            TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .UpsertDeficientConditionGoalLibrary(library);
            var dto = DeficientConditionGoalDtos.CulvDurationN();
            var dtos = new List<DeficientConditionGoalDTO> { dto };

            // Act
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dtos, libraryId);

            // Assert
            var dtosAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(libraryId);
            var dtoAfter = dtosAfter.Single();
            ObjectAssertions.EquivalentExcluding(dto, dtoAfter, x => x.CriterionLibrary);
        }

        [Fact]
        public void Delete_LibraryDoesNotExist_DoesNotThrow()
        {
            Setup();

            // Act and assert
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(Guid.NewGuid());
        }

        [Fact]
        public void GetDeficientConditionGoalLibrariesNoChildren_DoesNotGetDeficientConditionGoals()
        {
            // Arrange
            Setup();
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            SetupLibraryForGet(libraryId, goalId);

            // Act
            var dtos = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren();

            // Assert

            var ourDto = dtos.Single(dto => dto.Id == libraryId);
            Assert.Empty(ourDto.DeficientConditionGoals);
        }

        [Fact]
        public void UpsertDeficientConditionGoals_LibraryInDbWithDeficientConditionGoal_Modifies()
        {
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            Setup();
            SetupLibraryForGet(libraryId, goalId);
            var library = TestDeficientConditionGoalLibrary(libraryId).ToDto();
            TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .UpsertDeficientConditionGoalLibrary(library);
            var dto = DeficientConditionGoalDtos.CulvDurationN();
            var dtos = new List<DeficientConditionGoalDTO> { dto };
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dtos, libraryId);
            var dto2 = DeficientConditionGoalDtos.CulvDurationN();
            dto2.Id = dto.Id;
            dto2.Name = "Updated name";
            var dtos2 = new List<DeficientConditionGoalDTO> { dto2 };
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dtos2, libraryId);

            var dtosAfter2 = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(libraryId);
            var dtoAfter2 = dtosAfter2.Single();
            ObjectAssertions.EquivalentExcluding(dto2, dtoAfter2, dto => dto.CriterionLibrary);
        }

        [Fact]
        public void DeleteDeficientGoalLibrary_LibraryInDbWithGoal_DeletesBoth()
        {
            // Arrange
            Setup();
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            SetupLibraryForGet(libraryId, goalId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var getResult = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren();
            var deficientConditionGoalLibraryDTO = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary = criterionLibrary;
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(deficientConditionGoalLibraryDTO.DeficientConditionGoals, deficientConditionGoalLibraryDTO.Id);
            Assert.True(TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.True(TestHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == goalId));

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);

            Assert.False(TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.False(TestHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == goalId));
        }

        //Scenarios
        [Fact]
        public void UpsertOrDeleteScenarioDeficientConditionGoals_GoalIsInDbButNotInList_DeletesFromDb()
        {
            Setup();
            var goalId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            // Arrange          
            SetupScenarioGoalsForGet(simulationId, goalId);
            var dtos = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);

            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

            var deleteId = Guid.NewGuid();
            TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Add(new ScenarioDeficientConditionGoalEntity
            {
                Id = deleteId,
                SimulationId = simulationId,
                AttributeId = attribute.Id,
                Name = "Deleted"
            });
            TestHelper.UnitOfWork.Context.SaveChanges();

            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var indexToDelete = localScenarioDeficientGoals.FindIndex(g => g.Id == deleteId);
            var cloneLocalGoals = localScenarioDeficientGoals.ToList();
            cloneLocalGoals.RemoveAt(indexToDelete);
            Assert.True(
                 TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Any(_ => _.Id == deleteId));

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(cloneLocalGoals, simulationId);

            Assert.False(
                TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Any(_ => _.Id == deleteId));
        }

        [Fact]
        public void UpsertOrDeleteScenarioDeficientConditionGoals_ListContainsGoalNotInDb_Adds()
        {
            Setup();
            var simulationId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            // Arrange
            SetupScenarioGoalsForGet(simulationId, goalId);

            var attribute2 = TestHelper.UnitOfWork.Context.Attribute.First();

            var newGoalId2 = Guid.NewGuid();
            var addedGoal2 = new DeficientConditionGoalDTO
            {
                Id = newGoalId2,
                Attribute = attribute2.Name,
                Name = "New"
            };
            var existingRows = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(
                simulationId);
            existingRows.Add(addedGoal2);

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(existingRows, simulationId);

            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var localNewDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Name == "New");
            var serverNewDeficientGoal = localScenarioDeficientGoals.FirstOrDefault(_ => _.Id == newGoalId2);
            Assert.NotNull(serverNewDeficientGoal);
            Assert.Equal(localNewDeficientGoal.Attribute, serverNewDeficientGoal.Attribute);
        }

        [Fact]
        public void UpsertOrDeleteScenarioDeficientConditionGoals_UpdatedGoalInList_UpdatesInDb()
        {
            Setup();
            var simulationId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            // Arrange
            SetupScenarioGoalsForGet(simulationId, goalId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();

            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var goalToUpdate = localScenarioDeficientGoals.First();
            var updatedGoalId = goalToUpdate.Id;
            goalToUpdate.Name = "Updated";
            goalToUpdate.CriterionLibrary = criterionLibrary;

            // Act
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(localScenarioDeficientGoals, simulationId);

            // Assert
            var serverScenarioDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetScenarioDeficientConditionGoals(simulationId);
            Assert.Equal(serverScenarioDeficientConditionGoals.Count, serverScenarioDeficientConditionGoals.Count);

            var localScenarioDeficientGoalsAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);

            var localUpdatedDeficientGoalAfter = localScenarioDeficientGoalsAfter.Single(_ => _.Id == updatedGoalId);
            var serverUpdatedDeficientGoal = serverScenarioDeficientConditionGoals
                .FirstOrDefault(_ => _.Id == updatedGoalId);
            Assert.Equal(localUpdatedDeficientGoalAfter.Name, serverUpdatedDeficientGoal.Name);
            Assert.Equal(localUpdatedDeficientGoalAfter.Attribute, serverUpdatedDeficientGoal.Attribute);
            Assert.Equal(localUpdatedDeficientGoalAfter.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void GetGoalsForScenario_GoalInDb_GetsTheGoal()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            SetupScenarioGoalsForGet(simulationId, goalId);

            // Act
            var dtos = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);

            // Assert
            var dto = dtos.Single(_ => _.Id == goalId);
            Assert.NotNull(dto);
        }

        [Fact]
        public async Task CreateDeficientConditionGoalLibraryWithUser_Does()
        {
            var libraryName = RandomStrings.WithPrefix("DeficientConditionGoalLibrary");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var libraryDto = DeficientConditionGoalLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName);
            var userDto = DeficientConditionGoalLibraryUserTestSetup.CreateLibraryUserDto(user.Id);
            var userDtos = new List<LibraryUserDTO> { userDto };

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(libraryDto.Id, userDtos);

            var usersAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryAccess(libraryDto.Id, user.Id);
            var accessAfter = usersAfter.Access;
            Assert.Equal(user.Id, accessAfter.UserId);
        }
        [Fact]
        public async Task UpdateDeficientConditionGoalLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = DeficientConditionGoalLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            DeficientConditionGoalLibraryUserTestSetup.SetUsersOfDeficientConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }

        [Fact]
        public async Task UpdateDeficientConditionGoalLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = DeficientConditionGoalLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            DeficientConditionGoalLibraryUserTestSetup.SetUsersOfDeficientConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }

        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = DeficientConditionGoalLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            DeficientConditionGoalLibraryUserTestSetup.SetUsersOfDeficientConditionGoalLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }
    }
}
