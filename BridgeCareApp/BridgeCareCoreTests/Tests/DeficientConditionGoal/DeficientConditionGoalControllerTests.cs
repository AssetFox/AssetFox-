﻿using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DeficientConditionGoalControllerTests
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private static DeficientConditionGoalController CreateController(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var deficientConditionGoalService = new DeficientConditionGoalPagingService(mockUnitOfWork.Object);
            var controller = new DeficientConditionGoalController(
                security.Object,
                mockUnitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                claimHelper.Object,
                deficientConditionGoalService
                );
            return controller;
        }

        [Fact]
        public async Task GetDeficientConditionGoalLibrariesNoChildren_GetsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var library = DeficientConditionGoalLibraryDtos.Empty();
            var libraries = new List<DeficientConditionGoalLibraryDTO> { library };
            repo.Setup(r => r.GetDeficientConditionGoalLibrariesNoChildren()).Returns(libraries);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            ActionResultAssertions.Singleton(library, result);
        }

        [Fact]
        public async Task UpsertDeficientConditionGoalLibrary_NewLibrary_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var library = DeficientConditionGoalLibraryDtos.Empty();
            var request = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                Library = library,
                IsNewLibrary = true,
            };
            var libraryDoesNotExist = LibraryAccessModels.LibraryDoesNotExist();
            repo.SetupGetLibraryAccess(library.Id, libraryDoesNotExist);
            // Act
            var result = await controller
                .UpsertDeficientConditionGoalLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryInvocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.UpsertDeficientConditionGoalLibraryAndGoals));
            Assert.Equal(library, libraryInvocation.Arguments[0]);
            Assert.Empty(library.DeficientConditionGoals);
        }

        [Fact]
        public async Task DeleteDeficientConditionGoalLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.DeleteDeficientConditionGoalLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
        }

        [Fact]
        public async Task DeficientConditionGoalLibraries_GetDeficientConditionGoalLibrariesNoChildrenOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var library = DeficientConditionGoalLibraryDtos.Empty();
            var libaries = new List<DeficientConditionGoalLibraryDTO> { library };
            repo.Setup(r => r.GetDeficientConditionGoalLibrariesNoChildren()).Returns(libaries);

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            ActionResultAssertions.Singleton(library, result);
        }

        [Fact]
        public async Task UpsertConditionGoalLibrary_PassesLibraryAndGoalsToRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var user = UserDtos.Admin();
            var controller = CreateController(unitOfWork);
            var libraryDto = DeficientConditionGoalLibraryDtos.Empty();
            var libraryId = libraryDto.Id;
            var goalId = Guid.NewGuid();
            var goalDto = DeficientConditionGoalDtos.CulvDurationN(goalId);
            var goalDto2 = DeficientConditionGoalDtos.CulvDurationN(goalId);
            libraryDto.Description = "Updated Description";
            goalDto2.Name = "Updated Name";
            repo.Setup(r => r.GetDeficientConditionGoalsByLibraryId(libraryId)).Returns(new List<DeficientConditionGoalDTO> { goalDto });

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { goalDto2 }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = libraryDto,
                SyncModel = sync
            };
            var libraryUser = LibraryUserDtos.Modify(user.Id);
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(user.Id, libraryUser);
            repo.SetupGetLibraryAccess(libraryId, libraryExists);

            // Act
            var result = await controller.UpsertDeficientConditionGoalLibrary(libraryRequest);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryInvocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.UpsertDeficientConditionGoalLibraryAndGoals));
            var upsertedLibrary = libraryInvocation.Arguments[0] as DeficientConditionGoalLibraryDTO;
            var upsertedGoals = upsertedLibrary.DeficientConditionGoals;
            Assert.Equal("Updated Name", upsertedGoals[0].Name);
            Assert.Equal(libraryDto, libraryInvocation.Arguments[0]);
        }

        //Scenarios
        [Fact]
        public async Task UpsertScenarioDeficientConditionGoals_GoalDeletedInSyncModel_AsksRepositoryToUpsertWithoutGoal()
        {
            // Arrange          
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);

            var deleteId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var sync3 = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                RowsForDeletion = new List<Guid>() { deleteId }
            };
            var goal = DeficientConditionGoalDtos.CulvDurationN(deleteId);
            var goals = new List<DeficientConditionGoalDTO> { goal };
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(simulationId)).Returns(goals);

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync3);

            // Assert
            var invocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.UpsertOrDeleteScenarioDeficientConditionGoals));
            Assert.Equal(simulationId, invocation.Arguments[1]);
            var invocationDtos = invocation.Arguments[0] as List<DeficientConditionGoalDTO>;
            Assert.Empty(invocationDtos);
        }

        [Fact]
        public async Task UpsertScenarioDeficientConditionGoals_WithGoalToAdd_PassesAddedGoalToRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var newGoalId2 = Guid.NewGuid();
            var addedGoal2 = DeficientConditionGoalDtos.CulvDurationN(newGoalId2);
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(simulationId)).Returns(new List<DeficientConditionGoalDTO>());
            var sync2 = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                AddedRows = new List<DeficientConditionGoalDTO>() { addedGoal2 },
            };

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync2);

            // Assert
            var invocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.UpsertOrDeleteScenarioDeficientConditionGoals));
            Assert.Equal(simulationId, invocation.Arguments[1]);
            ObjectAssertions.Singleton(addedGoal2, invocation.Arguments[0]);
        }

        [Fact]
        public async Task UpsertScenarioDeficientConditionGoals_WithGoalToUpdate_PassesGoalToRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var originalGoal = DeficientConditionGoalDtos.CulvDurationN(goalId);
            var updateGoal = DeficientConditionGoalDtos.CulvDurationN(goalId);
            updateGoal.Name = "Update name";
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(simulationId)).Returns(new List<DeficientConditionGoalDTO> { originalGoal });
            var sync2 = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { updateGoal },
            };

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulationId, sync2);

            // Assert
            var invocation = repo.SingleInvocationWithName(nameof(IDeficientConditionGoalRepository.UpsertOrDeleteScenarioDeficientConditionGoals));
            Assert.Equal(simulationId, invocation.Arguments[1]);
            var argumentZero = (List<DeficientConditionGoalDTO>)invocation.Arguments[0];
            Assert.Equal(updateGoal, argumentZero.Single());
        }

        [Fact]
        public async Task GetScenarioDeficientConditionGoalPage_PassesRequestToRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var goal = DeficientConditionGoalDtos.CulvDurationN(simulationId);
            var goals = new List<DeficientConditionGoalDTO> { goal };
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(simulationId)).Returns(goals);

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
