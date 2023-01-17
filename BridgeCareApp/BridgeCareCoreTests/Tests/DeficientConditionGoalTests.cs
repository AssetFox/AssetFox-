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
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using MathNet.Numerics.Statistics.Mcmc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework.Internal.Execution;
using Xunit;
using Microsoft.EntityFrameworkCore;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCoreTests.Helpers;

namespace BridgeCareCoreTests.Tests
{
    public class DeficientConditionGoalTests
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private static DeficientConditionGoalController Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DeficientConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService,
                accessor, _mockClaimHelper.Object,
                new DeficientConditionGoalPagingService(TestHelper.UnitOfWork));
            return controller;
        }
        private DeficientConditionGoalController CreateTestController(List<string> uClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(uClaims);
            var controller = new DeficientConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService,
                accessor, _mockClaimHelper.Object,
                 new DeficientConditionGoalPagingService(TestHelper.UnitOfWork));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
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
            var goal = TestDeficientConditionGoal(libraryId);
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
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = Setup();

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var controller = Setup();
            SetupLibraryForGet(libraryId, goalId);
            var request = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>();
            request.Library = TestDeficientConditionGoalLibrary(libraryId).ToDto();
            // Act
            var result = await controller
                .UpsertDeficientConditionGoalLibrary(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var controller = Setup();

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllDeficientConditionGoalLibraries()
        {
            // Arrange
            var controller = Setup();
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            SetupLibraryForGet(libraryId, goalId);

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<DeficientConditionGoalLibraryDTO>));
            var ourDto = dtos.Single(dto => dto.Id == libraryId);
            Assert.Empty(ourDto.DeficientConditionGoals);
        }

        [Fact]
        public async Task ShouldModifyDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var goalEntity = SetupLibraryForGet(libraryId, goalId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var libraryDto = TestDeficientConditionGoalLibrary(libraryId).ToDto();
            var goalDto = goalEntity.ToDto();
            libraryDto.Description = "Updated Description";
            goalDto.Name = "Updated Name";
            goalDto.CriterionLibrary = criterionLibrary;

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { goalDto }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = libraryDto,
                SyncModel = sync
            };

            // Act
            await controller.UpsertDeficientConditionGoalLibrary(libraryRequest);

            // Assert

            var modifiedDto = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            Assert.Equal(libraryDto.Description, modifiedDto.Description);
            Assert.Equal(goalDto.Attribute,
                modifiedDto.DeficientConditionGoals[0].Attribute);
            // below asserts all fail as of 6/6/2022:
            //Assert.Single(modifiedDto.AppliedScenarioIds);
            //  Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);
            // to fix the above, explicitly create a Simulation somewhere, perhaps in setup, then use its id and check against said id in the assert.

            //Assert.Equal(dto.DeficientConditionGoals[0].Name, modifiedDto.DeficientConditionGoals[0].Name);
            //Assert.Equal(dto.DeficientConditionGoals[0].CriterionLibrary.Id,
            //   modifiedDto.DeficientConditionGoals[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldDeleteDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            SetupLibraryForGet(libraryId, goalId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var getResult = await controller.DeficientConditionGoalLibraries();
            var deficientConditionGoalLibraryDTO = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary = criterionLibrary;

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { deficientConditionGoalLibraryDTO.DeficientConditionGoals[0] }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = deficientConditionGoalLibraryDTO,
                SyncModel = sync
            };

            await controller.UpsertDeficientConditionGoalLibrary(libraryRequest);

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.True(!TestHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == goalId));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryDeficientConditionGoal.Any(_ =>
                    _.DeficientConditionGoalId == goalId));
        }


        //Scenarios
        [Fact]
        public async Task ShouldDeleteScenarioDeficientConditionGoalData()
        {
            var controller = Setup();
            var goalId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            // Arrange          
            SetupScenarioGoalsForGet(simulationId, goalId);
            var getResult = await controller.GetScenarioDeficientConditionGoals(simulationId);
            var dtos = (List<DeficientConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<DeficientConditionGoalDTO>));

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

            var localScenarioDeficientGoals3 = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var indexToDelete = localScenarioDeficientGoals3.FindIndex(g => g.Id == deleteId);

            var sync3 = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                RowsForDeletion = new List<Guid>() { deleteId }
            };

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync3);

            // Assert

            Assert.False(
                TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Any(_ => _.Id == deleteId));
        }

        [Fact]
        public async Task ShouldAddScenarioDeficientConditionGoalData()
        {
            var controller2 = Setup();
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

            var sync2 = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                AddedRows = new List<DeficientConditionGoalDTO>() { addedGoal2 },
            };

            // Act
            await controller2.UpsertScenarioDeficientConditionGoals(simulationId, sync2);

            // Assert
            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var localNewDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Name == "New");
            var serverNewDeficientGoal = localScenarioDeficientGoals.FirstOrDefault(_ => _.Id == newGoalId2);
            Assert.NotNull(serverNewDeficientGoal);
            Assert.Equal(localNewDeficientGoal.Attribute, serverNewDeficientGoal.Attribute);
        }

        [Fact]
        public async Task ShouldUpdateScenarioDeficientConditionGoalData()
        {
            var controller = Setup();
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


            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { goalToUpdate }
            };

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync);

            // Assert
            var serverScenarioDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetScenarioDeficientConditionGoals(simulationId);
            Assert.Equal(serverScenarioDeficientConditionGoals.Count, serverScenarioDeficientConditionGoals.Count);

            localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);


            var localUpdatedDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Id == updatedGoalId);
            var serverUpdatedDeficientGoal = serverScenarioDeficientConditionGoals
                .FirstOrDefault(_ => _.Id == updatedGoalId);
            Assert.Equal(localUpdatedDeficientGoal.Name, serverUpdatedDeficientGoal.Name);
            Assert.Equal(localUpdatedDeficientGoal.Attribute, serverUpdatedDeficientGoal.Attribute);
            Assert.Equal(localUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public async Task ShouldGetScenarioDeficientConditionGoalPageData()
        {
            var controller = Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            SetupScenarioGoalsForGet(simulationId, goalId);
            var goal = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId).Single(_ => _.Id == goalId);

            // Act
            var request = new PagingRequestModel<DeficientConditionGoalDTO>();
            var result = await controller.GetScenarioDeficientConditionGoalPage(simulationId, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<DeficientConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<DeficientConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single(_ => _.Id == goal.Id);
            Assert.Equal(goal.Id, dto.Id);
        }
    }
}
