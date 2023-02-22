using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.BudgetPriority;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DeficientConditionGoalPagingServiceTests
    {
        private DeficientConditionGoalPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new DeficientConditionGoalPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToUpdate_ReturnsUpdatedRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = DeficientConditionGoalDtos.CulvDurationN(goalId, criterionLibraryId);
            var dto2 = DeficientConditionGoalDtos.CulvDurationN(goalId, criterionLibraryId);
            dto2.Name = "Updated name";
            var dto3 = DeficientConditionGoalDtos.CulvDurationN(goalId, criterionLibraryId);
            dto3.Name = "Updated name";
            var dtos = new List<DeficientConditionGoalDTO> { dto1 };
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                UpdateRows = new List<DeficientConditionGoalDTO> { dto2 },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto3, resultDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToDelete_DeletesRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var dto = DeficientConditionGoalDtos.CulvDurationN(goalId);
            var dtos = new List<DeficientConditionGoalDTO> { dto };
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                RowsForDeletion = new List<Guid> { goalId },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetScenarioPage_RequestSecondPage_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = DeficientConditionGoalDtos.CulvDurationN();
            var dto2 = DeficientConditionGoalDtos.CulvDurationN(goalId, criterionLibraryId);
            var dto2Clone = DeficientConditionGoalDtos.CulvDurationN(goalId, criterionLibraryId);
            var dtos = new List<DeficientConditionGoalDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioDeficientConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>();
            var pagingRequest = new PagingRequestModel<DeficientConditionGoalDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 2,
            };

            var result = pagingService.GetScenarioPage(scenarioId, pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto2Clone, item);
        }

        [Fact]
        public void GetLibraryPage_NumberOfRowsGoesBeyondPageSize_TruncatesReturnedList()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = DeficientConditionGoalLibraryDtos.Empty(libraryId);
            var goal1 = DeficientConditionGoalDtos.CulvDurationN();
            var goal2 = DeficientConditionGoalDtos.CulvDurationN();
            repo.Setup(r => r.GetDeficientConditionGoalsByLibraryId(libraryId)).ReturnsList(goal1, goal2);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<DeficientConditionGoalDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedGoal = result.Items.Single();
            ObjectAssertions.Equivalent(goal1, returnedGoal);
        }

        [Fact]
        public void GetLibraryPage2_NumberOfRowsGoesBeyondPageSize_TruncatesReturnedList()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = DeficientConditionGoalRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = DeficientConditionGoalLibraryDtos.Empty(libraryId);
            var goal1 = DeficientConditionGoalDtos.CulvDurationN();
            var goal2 = DeficientConditionGoalDtos.CulvDurationN();
            repo.Setup(r => r.GetDeficientConditionGoalsByLibraryId(libraryId)).ReturnsList(goal1, goal2);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<DeficientConditionGoalDTO>
            {
                Page = 2,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedGoal = result.Items.Single();
            ObjectAssertions.Equivalent(goal2, returnedGoal);
        }
        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithAddedRow_HasRowWithFreshId()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = DeficientConditionGoalLibraryDtos.Empty(libraryId);
            var goal = DeficientConditionGoalDtos.CulvDurationN(Guid.Empty, Guid.Empty);
            goal.CriterionLibrary = null;
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                AddedRows = new List<DeficientConditionGoalDTO> { goal },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedGoal = result.Single();
            Assert.NotEqual(Guid.Empty, returnedGoal.Id);
            Assert.Null(returnedGoal.CriterionLibrary);
        }
        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithAddedRowWithCriterionLibrary_HasRowWithFreshIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = DeficientConditionGoalLibraryDtos.Empty(libraryId);
            var goal = DeficientConditionGoalDtos.CulvDurationN(Guid.Empty, Guid.Empty);
            var syncModel = new PagingSyncModel<DeficientConditionGoalDTO>
            {
                AddedRows = new List<DeficientConditionGoalDTO> { goal },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedGoal = result.Single();
            Assert.NotEqual(Guid.Empty, returnedGoal.Id);
            Assert.NotEqual(Guid.Empty, returnedGoal.CriterionLibrary.Id);
        }
    }
}
