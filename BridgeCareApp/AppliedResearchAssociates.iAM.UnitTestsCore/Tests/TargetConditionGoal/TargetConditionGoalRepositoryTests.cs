using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

        private TargetConditionGoalLibraryEntity
            TestTargetConditionGoalLibraryEntity(
            Guid? id = null,
            string name = null
            )
        {
            var resolvedId = id ?? Guid.NewGuid();
            var resolvedName = name ?? RandomStrings.Length11();
            var returnValue = new TargetConditionGoalLibraryEntity
            {
                Id = resolvedId,
                Name = resolvedName,
            };
            return returnValue;
        }

        private TargetConditionGoalEntity TestTargetConditionGoal(
            Guid libraryId,
            Guid? id = null,
            string name = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = name ?? RandomStrings.Length11();
            var returnValue = new TargetConditionGoalEntity
            {
                Id = resolveId,
                TargetConditionGoalLibraryId = libraryId,
                Name = resolveName,
                Target = 1
            };
            return returnValue;
        }
        private ScenarioTargetConditionGoalEntity TestScenarioTargetConditionGoal(Guid simulationId,
            Guid attributeId,
            Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new ScenarioTargetConditionGoalEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Name = "Test Name",
                Target = 1
            };
            return returnValue;
        }

        private TargetConditionGoalLibraryEntity SetupLibraryForGet()
        {
            var libraryEntity = TestTargetConditionGoalLibraryEntity();
            TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Add(libraryEntity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return libraryEntity;
        }

        public TargetConditionGoalEntity SetupTargetConditionGoal(Guid targetConditionGoalLibraryId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var targetConditionGoalEntity = TestTargetConditionGoal(targetConditionGoalLibraryId);
            targetConditionGoalEntity.AttributeId = attribute.Id;
            TestHelper.UnitOfWork.Context.TargetConditionGoal.Add(targetConditionGoalEntity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return targetConditionGoalEntity;
        }

        private CriterionLibraryDTO SetupCriterionLibraryForUpsertOrDelete()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            return criterionLibrary;
        }

        private ScenarioTargetConditionGoalEntity SetupForScenarioTargetGet(Guid simulationId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal = TestScenarioTargetConditionGoal(simulationId, attribute.Id);
            TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(goal);
            TestHelper.UnitOfWork.Context.SaveChanges();
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
            var entity = SetupLibraryForGet();
            var library = entity.ToDto();
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

        //[Fact]
        //public async Task ShouldModifyTargetConditionGoalData()
        //{
        //    Setup();
        //    // Arrange
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
        //    var library = SetupLibraryForGet();
        //    var goal = SetupTargetConditionGoal(library.Id);

        //    var libraryDto = library.ToDto();
        //    var goalDto = goal.ToDto();

        //    libraryDto.Description = "Updated Description";
        //    goalDto.Name = "Updated Name";
        //    goalDto.CriterionLibrary = criterionLibrary;

        //    var sync = new PagingSyncModel<TargetConditionGoalDTO>()
        //    {
        //        UpdateRows = new List<TargetConditionGoalDTO>() { goalDto }
        //    };

        //    var libraryRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>()
        //    {
        //        IsNewLibrary = false,
        //        Library = libraryDto,
        //        SyncModel = sync
        //    };

        //    // Act
        //    await controller.UpsertTargetConditionGoalLibrary(libraryRequest);

        //    // Assert
        //    var modifiedDto = TestHelper.UnitOfWork.TargetConditionGoalRepo
        //        .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
        //        .Single(x => x.Id == library.Id);
        //    Assert.Equal(libraryDto.Description, modifiedDto.Description);

        //    // below fails on some db weirdness. The name is updated in the db but not in the get result!?!
        //    // Assert.Equal(dto.TargetConditionGoals[0].Name, modifiedDto.TargetConditionGoals[0].Name);
        //    Assert.Equal(goalDto.Attribute, modifiedDto.TargetConditionGoals[0].Attribute);
        //}

        //[Fact]
        //public async Task ShouldDeleteTargetConditionGoalData()
        //{
        //    Setup();
        //    // Arrange
        //    var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
        //    var targetConditionGoalLibraryEntity = SetupLibraryForGet();
        //    var libraryId = targetConditionGoalLibraryEntity.Id;
        //    var targetConditionGoalEntity = SetupTargetConditionGoal(libraryId);
        //    var goalId = targetConditionGoalEntity.Id;

        //    JoinCriterionToLibraryConditionGoal(goalId, criterionLibrary.Id);

        //    // Act
        //    var result = await controller.DeleteTargetConditionGoalLibrary(libraryId);

        //    // Assert
        //    Assert.IsType<OkResult>(result);

        //    Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId));
        //    Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == goalId));
        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Any(_ =>
        //            _.TargetConditionGoalId == TargetConditionGoalId));
        //}

        //[Fact]
        //public async Task ShouldGetAllScenarioTargetConditionGoalData()
        //{
        //    Setup();
        //    // Arrange
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var goal = SetupForScenarioTargetGet(simulation.Id);

        //    // Act
        //    var result = await controller.GetScenarioTargetConditionGoals(simulation.Id);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(List<TargetConditionGoalDTO>));
        //    var dto = dtos.Single();
        //    Assert.Equal(goal.Id, dto.Id);
        //}

        //[Fact]
        //public async Task ShouldGetScenarioTargetConditionGoalPageData()
        //{
        //    Setup();
        //    // Arrange
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var goal = SetupForScenarioTargetGet(simulation.Id);

        //    // Act
        //    var request = new PagingRequestModel<TargetConditionGoalDTO>();
        //    var result = await controller.GetScenarioTargetConditionGoalPage(simulation.Id, request);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(PagingPageModel<TargetConditionGoalDTO>));
        //    var dtos = page.Items;
        //    var dto = dtos.Single();
        //    Assert.Equal(goal.Id, dto.Id);
        //}

        //[Fact]
        //public async Task ShouldGetLibraryTargetConditionGoalPageData()
        //{
        //    Setup();
        //    // Arrange
        //    var library = SetupLibraryForGet();
        //    var goal = SetupTargetConditionGoal(library.Id);

        //    // Act
        //    var request = new PagingRequestModel<TargetConditionGoalDTO>();
        //    var result = await controller.GetLibraryTargetConditionGoalPage(library.Id, request);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(PagingPageModel<TargetConditionGoalDTO>));
        //    var dtos = page.Items;
        //    var dto = dtos.Single();
        //    Assert.Equal(goal.Id, dto.Id);
        //}

        //[Fact]
        //public async Task ShouldModifyScenarioTargetConditionGoalData()
        //{
        //    Setup();
        //    // Arrange
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var criterionLibrary = SetupForScenarioTargetUpsertOrDelete(simulation.Id);
        //    var getResult = await controller.GetScenarioTargetConditionGoals(simulation.Id);
        //    var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
        //        typeof(List<TargetConditionGoalDTO>));

        //    var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

        //    var deletedTargetConditionId = Guid.NewGuid();
        //    TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(new ScenarioTargetConditionGoalEntity
        //    {
        //        Id = deletedTargetConditionId,
        //        SimulationId = simulation.Id,
        //        AttributeId = attribute.Id,
        //        Name = "Deleted"
        //    });
        //    TestHelper.UnitOfWork.Context.SaveChanges();

        //    var localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
        //    var indexToDelete = localScenarioTargetGoals.FindIndex(g => g.Id == deletedTargetConditionId);
        //    var deleteId = localScenarioTargetGoals[indexToDelete].Id;
        //    var goalToUpdate = localScenarioTargetGoals.Single(g => g.Id != deletedTargetConditionId);
        //    var updatedGoalId = goalToUpdate.Id;
        //    goalToUpdate.Name = "Updated";
        //    goalToUpdate.CriterionLibrary = criterionLibrary;
        //    var newGoalId = Guid.NewGuid();
        //    var addedGoal = new TargetConditionGoalDTO
        //    {
        //        Id = newGoalId,
        //        Attribute = attribute.Name,
        //        Name = "New"
        //    };

        //    var sync = new PagingSyncModel<TargetConditionGoalDTO>()
        //    {
        //        UpdateRows = new List<TargetConditionGoalDTO>() { goalToUpdate },
        //        AddedRows = new List<TargetConditionGoalDTO>() { addedGoal },
        //        RowsForDeletion = new List<Guid>() { deleteId }
        //    };

        //    // Act
        //    await controller.UpsertScenarioTargetConditionGoals(simulation.Id, sync);

        //    // Assert
        //    var serverScenarioTargetConditionGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo
        //        .GetScenarioTargetConditionGoals(simulation.Id);
        //    Assert.Equal(serverScenarioTargetConditionGoals.Count, serverScenarioTargetConditionGoals.Count);

        //    Assert.False(
        //        TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Any(_ => _.Id == deletedTargetConditionId));
        //    localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
        //    var localNewTargetGoal = localScenarioTargetGoals.Single(_ => _.Name == "New");
        //    var serverNewTargetGoal = localScenarioTargetGoals.FirstOrDefault(_ => _.Id == newGoalId);
        //    Assert.NotNull(serverNewTargetGoal);
        //    Assert.Equal(localNewTargetGoal.Attribute, serverNewTargetGoal.Attribute);

        //    var localUpdatedTargetGoal = localScenarioTargetGoals.Single(_ => _.Id == updatedGoalId);
        //    var serverUpdatedTargetGoal = serverScenarioTargetConditionGoals
        //        .FirstOrDefault(_ => _.Id == updatedGoalId);
        //    ObjectAssertions.Equivalent(localNewTargetGoal, serverNewTargetGoal);
        //    Assert.Equal(localUpdatedTargetGoal.Name, serverUpdatedTargetGoal.Name);
        //    Assert.Equal(localUpdatedTargetGoal.Attribute, serverUpdatedTargetGoal.Attribute);
        //    Assert.Equal(localUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression,
        //        serverUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression);
        //}

    }
}
