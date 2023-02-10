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

        [Fact]
        public async Task TargetConditionGoalLibraries_GrabsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var libraryDto = TargetConditionGoalLibraryDtos.Dto();
            var libraryDtos = new List<TargetConditionGoalLibraryDTO> { libraryDto };
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
        public async Task UpsertTargetConditionGoalLibrary_LibraryExists_UpsertsLibraryAndGoals()
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
        public async Task GetScenarioTargetConditionGoals_GetsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var goal = TargetConditionGoalDtos.Dto("attribute");
            repo.Setup(r => r.GetScenarioTargetConditionGoals(simulationId)).ReturnsList(goal);

            // Act
            var result = await controller.GetScenarioTargetConditionGoals(simulationId);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var dtos = value as List<TargetConditionGoalDTO>;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task GetScenarioTargetConditionGoalPage_GetsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var goal = TargetConditionGoalDtos.Dto("attribute");
            repo.Setup(r => r.GetScenarioTargetConditionGoals(simulationId)).ReturnsList(goal);
            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetScenarioTargetConditionGoalPage(simulationId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var page = value as PagingPageModel<TargetConditionGoalDTO>;
            var dtos = page.Items;
            var dto = dtos.Single();
            ObjectAssertions.Equivalent(goal, dto);
        }

        [Fact]
        public async Task GetLibraryTargetConditionGoalPage_GetsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            var goal = TargetConditionGoalDtos.Dto("attribute");
            repo.Setup(r => r.GetTargetConditionGoalsByLibraryId(libraryId)).ReturnsList(goal);

            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetLibraryTargetConditionGoalPage(libraryId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var page = value as PagingPageModel<TargetConditionGoalDTO>;
            var dtos = page.Items;
            var dto = dtos.Single();
            ObjectAssertions.Equivalent(goal, dto);
        }

        [Fact]
        public async Task UpsertScenarioTargetConditionGoals_AsksRepoToUpsertOrDeleteScenarioTargetConditionGoals()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var updateName = "Update";
            var updateId = Guid.NewGuid();
            var deleteName = "Delete";
            var deleteId = Guid.NewGuid();
            var goalToUpdate = TargetConditionGoalDtos.Dto("attribute", updateId, updateName);
            var goalToDelete = TargetConditionGoalDtos.Dto("attribute", deleteId, deleteName);
            repo.Setup(r => r.GetScenarioTargetConditionGoals(simulationId)).ReturnsList(goalToUpdate, goalToDelete);
            var updatedGoalId = goalToUpdate.Id;
            goalToUpdate.Name = "Updated";  
            var newGoalId = Guid.NewGuid();
            var addedGoal = new TargetConditionGoalDTO
            {
                Id = newGoalId,
                Attribute = "attribute",
                Name = "New"
            };
            var sync = new PagingSyncModel<TargetConditionGoalDTO>()
            {
                UpdateRows = new List<TargetConditionGoalDTO>() { goalToUpdate },
                AddedRows = new List<TargetConditionGoalDTO>() { addedGoal},
                RowsForDeletion = new List<Guid>() { deleteId }
            };

            // Act
            await controller.UpsertScenarioTargetConditionGoals(simulationId, sync);

            var repoCall = repo.SingleInvocationWithName(nameof(ITargetConditionGoalRepository.UpsertOrDeleteScenarioTargetConditionGoals));
            var castArgumentZero = repoCall.Arguments[0] as List<TargetConditionGoalDTO>;
            Assert.Equal(2, castArgumentZero.Count);
            var actualAddedGoal = castArgumentZero.Single(g => g.Id == newGoalId);
            var actualUpdatedGoal = castArgumentZero.Single(g => g.Id == updatedGoalId);
            ObjectAssertions.Equivalent(addedGoal, actualAddedGoal);
            ObjectAssertions.Equivalent(goalToUpdate, actualUpdatedGoal);
            Assert.Equal(simulationId, repoCall.Arguments[1]);
            // Assert
        }
    }
}
