using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class TargetConditionGoalPagingServiceTests
    {
        private TargetConditionGoalPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new TargetConditionGoalPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioTargetConditionGoals(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
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
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto1 = TargetConditionGoalDtos.Dto("attribute", limitId, "targetConditionGoal1");
            var dto2 = TargetConditionGoalDtos.Dto("attribute", limitId, "targetConditionGoal2");
            var dto2Clone = TargetConditionGoalDtos.Dto("attribute", limitId, "targetConditionGoal2");
            var dtos = new List<TargetConditionGoalDTO> { dto1 };
            repo.Setup(r => r.GetScenarioTargetConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
            {
                UpdateRows = new List<TargetConditionGoalDTO> { dto2 },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto2Clone, resultDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToDelete_DeletesRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto = TargetConditionGoalDtos.Dto("attribute", limitId);
            var dtos = new List<TargetConditionGoalDTO> { dto };
            repo.Setup(r => r.GetScenarioTargetConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
            {
                RowsForDeletion = new List<Guid> { limitId },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetScenarioPage_RequestSecondPage_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto1 = TargetConditionGoalDtos.Dto("attribute1");
            var dto2 = TargetConditionGoalDtos.Dto("attribute2", limitId, "targetConditionGoal2");
            var dto2Clone = TargetConditionGoalDtos.Dto("attribute2", limitId, "targetConditionGoal2");
            var dtos = new List<TargetConditionGoalDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioTargetConditionGoals(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>();
            var pagingRequest = new PagingRequestModel<TargetConditionGoalDTO>
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
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var limit1 = TargetConditionGoalDtos.Dto("attribute1");
            var limit2 = TargetConditionGoalDtos.Dto("attribute1");
            repo.Setup(r => r.GetTargetConditionGoalsByLibraryId(libraryId)).ReturnsList(limit1, limit2);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<TargetConditionGoalDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedLimit = result.Items.Single();
            ObjectAssertions.Equivalent(limit1, returnedLimit);
        }

        [Fact]
        public void GetLibraryPage2_NumberOfRowsGoesBeyondPageSize_SkipsPage1()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = TargetConditionGoalRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = TargetConditionGoalLibraryDtos.Dto(libraryId);
            var limit1 = TargetConditionGoalDtos.Dto("attribute1");
            var limit2 = TargetConditionGoalDtos.Dto("attribute2");
            repo.Setup(r => r.GetTargetConditionGoalsByLibraryId(libraryId)).ReturnsList(limit1, limit2);
            var syncModel = new PagingSyncModel<TargetConditionGoalDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<TargetConditionGoalDTO>
            {
                Page = 2,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedLimit = result.Items.Single();
            ObjectAssertions.Equivalent(limit2, returnedLimit);
        }
    }
}
