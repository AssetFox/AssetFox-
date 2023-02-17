using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Xunit;
using Moq;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public class TreatmentPagingServiceTests
    {
        public TreatmentPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new TreatmentPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            selectableTreatmentRepo.Setup(s => s.GetScenarioSelectableTreatments(simulationId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<TreatmentDTO>();

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            Assert.Empty(result);
        }


        [Fact]
        public void GetScenarioDataset_RepoReturnsARow_ReturnsRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetScenarioSelectableTreatments(simulationId)).ReturnsList(dto);
            var syncModel = new PagingSyncModel<TreatmentDTO>();

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            ObjectAssertions.Equivalent(dtoClone, returnedDto);
        }

        [Fact]
        public void GetScenarioDataset_LibraryIdInRequest_RowIdsAreInitialized()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetSelectableTreatments(libraryId)).ReturnsList(dto);
            var syncModel = new PagingSyncModel<TreatmentDTO>
            {
                LibraryId = libraryId,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            ObjectAssertions.EquivalentExcluding(dtoClone, returnedDto, x => x.Id);
        }

    }
}
