using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class RemainingLifeLimitPagingServiceTests
    {
        private RemainingLifeLimitPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new RemainingLifeLimitPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
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
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto1 = RemainingLifeLimitDtos.Dto("attribute", limitId, 1);
            var dto2 = RemainingLifeLimitDtos.Dto("attribute", limitId, 2);
            var dto2Clone = RemainingLifeLimitDtos.Dto("attribute", limitId, 2);
            var dtos = new List<RemainingLifeLimitDTO> { dto1 };
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
            {
                UpdateRows = new List<RemainingLifeLimitDTO> { dto2 },
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
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto = RemainingLifeLimitDtos.Dto("attribute", limitId);
            var dtos = new List<RemainingLifeLimitDTO> { dto };
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
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
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto1 = RemainingLifeLimitDtos.Dto("attribute1");
            var dto2 = RemainingLifeLimitDtos.Dto("attribute2", limitId);
            var dto2Clone = RemainingLifeLimitDtos.Dto("attribute2", limitId);
            var dtos = new List<RemainingLifeLimitDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>();
            var pagingRequest = new PagingRequestModel<RemainingLifeLimitDTO>
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
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var limit1 = RemainingLifeLimitDtos.Dto("attribute1");
            var limit2 = RemainingLifeLimitDtos.Dto("attribute1");
            repo.Setup(r => r.GetRemainingLifeLimitsByLibraryId(libraryId)).ReturnsList(limit1, limit2);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<RemainingLifeLimitDTO>
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
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var limit1 = RemainingLifeLimitDtos.Dto("attribute1");
            var limit2 = RemainingLifeLimitDtos.Dto("attribute2");
            repo.Setup(r => r.GetRemainingLifeLimitsByLibraryId(libraryId)).ReturnsList(limit1, limit2);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<RemainingLifeLimitDTO>
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

        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithAddedRow_HasRowWithFreshIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var rule = RemainingLifeLimitDtos.Dto("attribute", Guid.Empty, 4);
            var syncModel = new PagingSyncModel<RemainingLifeLimitDTO>
            {
                AddedRows = new List<RemainingLifeLimitDTO> { rule },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedRule = result.Single();
            Assert.NotEqual(Guid.Empty, returnedRule.Id);
            Assert.NotEqual(Guid.Empty, returnedRule.CriterionLibrary.Id);
        }
    }
}
