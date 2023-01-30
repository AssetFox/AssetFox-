using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class TargetConditionGoalTests
    {
        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private TargetConditionGoalController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var pagingService = new TargetConditionGoalPagingService(unitOfWork.Object);
            var controller = new TargetConditionGoalController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                pagingService
                );
            return controller;
        }
        private TargetConditionGoalController SetupController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var controller = new TargetConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object,
                new TargetConditionGoalPagingService(TestHelper.UnitOfWork));
            return controller;
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
        public async Task TargetConditionGoalLibraries_GrabsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var libraryDto = TargetConditionGoalLibraryDtos.Dto();
            var libraryDtos = Lists.New(libraryDto);
            repo.Setup(r => r.GetTargetConditionGoalLibrariesNoChildren()).Returns(libraryDtos);
            var controller = CreateController(unitOfWork);
            // Act
            var result = await controller.TargetConditionGoalLibraries();

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(libraryDtos, value);
        }

        [Fact]
        public async Task UpsertTargetConditionGoalLibrary_CallsRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var libraryDto = TargetConditionGoalLibraryDtos.Dto();
            repo.Setup(r => r.GetTargetConditionGoalsByLibraryId(libraryDto.Id)).ReturnsEmptyList();
            var controller = CreateController(unitOfWork);
            var request = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>();
            request.Library = libraryDto;
            // Act
            var result = await controller
                .UpsertTargetConditionGoalLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(ITargetConditionGoalRepository.UpsertTargetConditionGoalLibrary));
            ObjectAssertions.Equivalent(libraryDto, libraryCall.Arguments[0]);
        }

        [Fact]
        public async Task DeleteTargetConditionGoalLibrary_CallsRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var libraryDto = TargetConditionGoalLibraryDtos.Dto();
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            // Act
            var result = await controller.DeleteTargetConditionGoalLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var repoCall = repo.SingleInvocationWithName(nameof(ITargetConditionGoalRepository.DeleteTargetConditionGoalLibrary));
            Assert.Equal(libraryId, repoCall.Arguments[0]);
        }

        [Fact]
        public async Task ShouldModifyTargetConditionGoalData()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            var libraryDto = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var goalDto = TargetConditionGoalDtos.Dto("attribute");
            repo.Setup(r => r.GetTargetConditionGoalsByLibraryId(libraryId)).ReturnsList(goalDto);

            libraryDto.Description = "Updated Description";
            goalDto.Name = "Updated Name";

            var sync = new PagingSyncModel<TargetConditionGoalDTO>()
            {
                UpdateRows = new List<TargetConditionGoalDTO>() { goalDto }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = libraryDto,
                SyncModel = sync
            };

            // Act
            var result = await controller.UpsertTargetConditionGoalLibrary(libraryRequest);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(ITargetConditionGoalRepository.UpsertTargetConditionGoalLibrary));
            var goalsCall = repo.SingleInvocationWithName(nameof(ITargetConditionGoalRepository.UpsertOrDeleteTargetConditionGoals));
            var upsertedLibrary = libraryCall.Arguments[0] as TargetConditionGoalLibraryDTO;
            Assert.Equal("Updated Description", upsertedLibrary.Description);
            var upsertedGoals = goalsCall.Arguments[0] as List<TargetConditionGoalDTO>;
            var upsertedGoal = upsertedGoals.Single();
            Assert.Equal("Updated Name", upsertedGoal.Name);
            Assert.Equal(libraryId, goalsCall.Arguments[1]);
        }

        [Fact]
        public async Task ShouldDeleteTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var targetConditionGoalLibraryEntity = SetupLibraryForGet();
            var libraryId = targetConditionGoalLibraryEntity.Id;
            var targetConditionGoalEntity = SetupTargetConditionGoal(libraryId);
            var goalId = targetConditionGoalEntity.Id;

            JoinCriterionToLibraryConditionGoal(goalId, criterionLibrary.Id);

            // Act
            var result = await controller.DeleteTargetConditionGoalLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == goalId));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Any(_ =>
                    _.TargetConditionGoalId == TargetConditionGoalId));
        }

        [Fact]
        public async Task ShouldGetAllScenarioTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var goal = SetupForScenarioTargetGet(simulation.Id);

            // Act
            var result = await controller.GetScenarioTargetConditionGoals(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TargetConditionGoalDTO>));
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTargetConditionGoalPageData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var goal = SetupForScenarioTargetGet(simulation.Id);

            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetScenarioTargetConditionGoalPage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<TargetConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetLibraryTargetConditionGoalPageData()
        {
            var controller = SetupController();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetLibraryTargetConditionGoalPage(library.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<TargetConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = SetupForScenarioTargetUpsertOrDelete(simulation.Id);
            var getResult = await controller.GetScenarioTargetConditionGoals(simulation.Id);
            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<TargetConditionGoalDTO>));

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
            var goalToUpdate = localScenarioTargetGoals.Single(g => g.Id!=deletedTargetConditionId);
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

            var sync = new PagingSyncModel<TargetConditionGoalDTO>()
            {
                UpdateRows = new List<TargetConditionGoalDTO>() { goalToUpdate },
                AddedRows = new List<TargetConditionGoalDTO>() { addedGoal},
                RowsForDeletion = new List<Guid>() { deleteId}
            };

            // Act
            await controller.UpsertScenarioTargetConditionGoals(simulation.Id, sync);

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
