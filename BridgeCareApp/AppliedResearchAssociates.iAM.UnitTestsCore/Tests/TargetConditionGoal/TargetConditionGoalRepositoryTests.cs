using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class TargetConditionGoalRepositoryTests
    {

        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");

        private void Setup()
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

        public TargetConditionGoalDTO SetupTargetConditionGoal(Guid targetConditionGoalLibraryId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var targetConditionGoal = TargetConditionGoalDtos.Dto(attribute.Name);
            var targetConditionGoals = new List<TargetConditionGoalDTO> { targetConditionGoal };
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(targetConditionGoals, targetConditionGoalLibraryId);
            return targetConditionGoal;
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
            var goals = Lists.New(goal);
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
            Setup();
            var library = SetupLibraryForGet();
            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo
                .UpsertTargetConditionGoalLibrary(library);
        }

        [Fact]
        public void DeleteTargetConditionGoalLibrary_LibraryDoesNotExist_DoesNotThrow()
        {
            Setup();
            // Act
            TestHelper.UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(Guid.NewGuid()); ;
        }

        [Fact]
        public void GetTargetConditionGoalLibrariesNoChildren_LibraryInDb_Gets()
        {
            Setup();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildren();
            var dto = dtos.Single(d => d.Id == library.Id);
        }

        [Fact]
        public void UpsertOrDeleteTargetConditionGoals_GoalInDbWithLibrary_ModifiesGoal()
        {
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var libraryDto = SetupLibraryForGet();
            var goalDto = SetupTargetConditionGoal(libraryDto.Id);
            goalDto.Name = "Updated Name";
            goalDto.CriterionLibrary = criterionLibrary;
            var updateRows = Lists.New(goalDto);

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
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
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
        public void DeleteTargetConditionGoalLibrary_DeletesGoalsAndData()
        {
            Setup();
            // Arrange
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var targetConditionGoalLibraryEntity = SetupLibraryForGet();
            var libraryId = targetConditionGoalLibraryEntity.Id;
            var targetConditionGoalEntity = SetupTargetConditionGoal(libraryId);
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
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
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
            Setup();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(library.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public void UpsertOrDeleteScenarioTargetConditionGoals_ScenarioInDbWithGoals_UpdatesOneDeletesAnother()
        {
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
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
            var goals = Lists.New(addedGoal, goalToUpdate);

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
    }
}
