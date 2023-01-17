using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

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
        public void ShouldDeleteDeficientConditionGoalData()
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

        ////Scenarios
        //[Fact]
        //public async Task ShouldDeleteScenarioDeficientConditionGoalData()
        //{
        //    var controller = Setup();
        //    var goalId = Guid.NewGuid();
        //    var simulationId = Guid.NewGuid();
        //    // Arrange          
        //    SetupScenarioGoalsForGet(simulationId, goalId);
        //    var getResult = await controller.GetScenarioDeficientConditionGoals(simulationId);
        //    var dtos = (List<DeficientConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
        //        typeof(List<DeficientConditionGoalDTO>));

        //    var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

        //    var deletedGoalId3 = Guid.NewGuid();
        //    TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Add(new ScenarioDeficientConditionGoalEntity
        //    {
        //        Id = deletedGoalId3,
        //        SimulationId = simulationId,
        //        AttributeId = attribute.Id,
        //        Name = "Deleted"
        //    });
        //    TestHelper.UnitOfWork.Context.SaveChanges();

        //    var localScenarioDeficientGoals3 = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
        //    var indexToDelete = localScenarioDeficientGoals3.FindIndex(g => g.Id == deletedGoalId3);
        //    var deleteId = localScenarioDeficientGoals3[indexToDelete].Id;

        //    var sync3 = new PagingSyncModel<DeficientConditionGoalDTO>()
        //    {
        //        RowsForDeletion = new List<Guid>() { deleteId }
        //    };

        //    // Act
        //    await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync3);

        //    // Assert

        //    Assert.False(
        //        TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Any(_ => _.Id == deletedGoalId3));
        //}

        //[Fact]
        //public async Task ShouldAddScenarioDeficientConditionGoalData()
        //{
        //    // wjwjwj deleting this test fixes the weird error
        //    var controller2 = Setup();
        //    var simulationId = Guid.NewGuid();
        //    var goalId = Guid.NewGuid();
        //    // Arrange
        //    SetupScenarioGoalsForGet(simulationId, goalId);

        //    var attribute2 = TestHelper.UnitOfWork.Context.Attribute.First();

        //    var newGoalId2 = Guid.NewGuid();
        //    var addedGoal2 = new DeficientConditionGoalDTO
        //    {
        //        Id = newGoalId2,
        //        Attribute = attribute2.Name,
        //        Name = "New"
        //    };

        //    var sync2 = new PagingSyncModel<DeficientConditionGoalDTO>()
        //    {
        //        AddedRows = new List<DeficientConditionGoalDTO>() { addedGoal2 },
        //    };

        //    // Act
        //    await controller2.UpsertScenarioDeficientConditionGoals(simulationId, sync2);

        //    // Assert
        //    var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
        //    var localNewDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Name == "New");
        //    var serverNewDeficientGoal = localScenarioDeficientGoals.FirstOrDefault(_ => _.Id == newGoalId2);
        //    Assert.NotNull(serverNewDeficientGoal);
        //    Assert.Equal(localNewDeficientGoal.Attribute, serverNewDeficientGoal.Attribute);
        //}

        //[Fact]
        //public async Task ShouldUpdateScenarioDeficientConditionGoalData()
        //{
        //    var controller = Setup();
        //    var simulationId = Guid.NewGuid();
        //    var goalId = Guid.NewGuid();
        //    // Arrange
        //    SetupScenarioGoalsForGet(simulationId, goalId);
        //    var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();

        //    var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

        //    var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
        //    var goalToUpdate = localScenarioDeficientGoals.First();
        //    var updatedGoalId = goalToUpdate.Id;
        //    goalToUpdate.Name = "Updated";
        //    goalToUpdate.CriterionLibrary = criterionLibrary;


        //    var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
        //    {
        //        UpdateRows = new List<DeficientConditionGoalDTO>() { goalToUpdate }
        //    };

        //    // Act
        //    await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync);

        //    // Assert
        //    var serverScenarioDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo
        //        .GetScenarioDeficientConditionGoals(simulationId);
        //    Assert.Equal(serverScenarioDeficientConditionGoals.Count, serverScenarioDeficientConditionGoals.Count);

        //    localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);


        //    var localUpdatedDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Id == updatedGoalId);
        //    var serverUpdatedDeficientGoal = serverScenarioDeficientConditionGoals
        //        .FirstOrDefault(_ => _.Id == updatedGoalId);
        //    Assert.Equal(localUpdatedDeficientGoal.Name, serverUpdatedDeficientGoal.Name);
        //    Assert.Equal(localUpdatedDeficientGoal.Attribute, serverUpdatedDeficientGoal.Attribute);
        //    Assert.Equal(localUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression,
        //        serverUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression);
        //}

        //[Fact]
        //public async Task ShouldGetScenarioDeficientConditionGoalPageData()
        //{
        //    var controller = Setup();
        //    // Arrange
        //    var libraryId = Guid.NewGuid();
        //    var goalId = Guid.NewGuid();
        //    var simulationId = Guid.NewGuid();
        //    SetupScenarioGoalsForGet(simulationId, goalId);
        //    var goal = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId).Single(_ => _.Id == goalId);

        //    // Act
        //    var request = new PagingRequestModel<DeficientConditionGoalDTO>();
        //    var result = await controller.GetScenarioDeficientConditionGoalPage(simulationId, request);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var page = (PagingPageModel<DeficientConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(PagingPageModel<DeficientConditionGoalDTO>));
        //    var dtos = page.Items;
        //    var dto = dtos.Single(_ => _.Id == goal.Id);
        //    Assert.Equal(goal.Id, dto.Id);
        //}
    }
}
