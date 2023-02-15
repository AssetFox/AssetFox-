using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public class PerformanceCurveServiceTests
    {
        [Fact]
        public void GetLibrarySyncedDataSet_NoCurvesInLibrary_ReturnsEmptyListOfCurves()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var libraryRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>
                {
                    LibraryId = libraryId,
                }
            };
            var curves = new List<PerformanceCurveDTO>();
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(curves);

            var dataset = service.GetSyncedLibraryDataset(libraryRequest);
            Assert.Empty(dataset);
        }

        [Fact]
        public void GetLibrarySyncedDataSet_OneCurveInLibrary_ReturnsTheCurve()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var libraryRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>
                {
                    LibraryId = libraryId,
                }
            };
            var curve = new PerformanceCurveDTO
            {
                Id = Guid.NewGuid(),
            };
            var curves = new List<PerformanceCurveDTO> { curve};
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(curves);

            var dataset = service.GetSyncedLibraryDataset(libraryRequest);
            var returnedCurve = dataset.Single();
            ObjectAssertions.Equivalent(curve, returnedCurve);
        }

        [Fact]
        public void GetLibrarySyncedDataSet_OneCurveInLibraryButMarkedForDeletion_DoesNotReturnTheCurve()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var curve = new PerformanceCurveDTO
            {
                Id = curveId,
            };
            var curves = new List<PerformanceCurveDTO> { curve };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                RowsForDeletion = new List<Guid> { curveId },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>
            {
                SyncModel = request,
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(curves);

            var dataset = service.GetSyncedLibraryDataset(upsertRequest);
            Assert.Empty(dataset);
        }

        [Fact]
        public void GetLibrarySyncedDataSet_NoCurvesInLibraryButOneMarkedForAdd_ReturnsTheCurve()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var curve = new PerformanceCurveDTO
            {
                Id = curveId,
            };
            var curves = new List<PerformanceCurveDTO> { curve };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                AddedRows = curves,
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>
            {
                SyncModel = request,
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());

            var dataset = service.GetSyncedLibraryDataset(upsertRequest);
            var returnedCurve = dataset.Single();
            ObjectAssertions.Equivalent(curve, returnedCurve);
        }
    }
}
