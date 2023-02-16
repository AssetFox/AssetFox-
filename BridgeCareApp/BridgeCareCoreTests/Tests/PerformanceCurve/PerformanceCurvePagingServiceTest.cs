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
    public class PerformanceCurvePagingServiceTests
    {
        [Fact]
        public void GetSyncedLibraryDataset_NoCurvesInLibrary_ReturnsEmptyListOfCurves()
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
        public void GetSyncedLibraryDataset_OneCurveInLibrary_ReturnsTheCurve()
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
            var curves = new List<PerformanceCurveDTO> { curve };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(curves);

            var dataset = service.GetSyncedLibraryDataset(libraryRequest);

            var returnedCurve = dataset.Single();
            ObjectAssertions.Equivalent(curve, returnedCurve);
        }

        [Fact]
        public void GetSyncedLibraryDataset_OneCurveInLibraryButMarkedForDeletion_DoesNotReturnTheCurve()
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
        public void GetSyncedLibraryDataset_NoCurvesInLibraryButOneMarkedForAdd_ReturnsTheCurve()
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

        [Fact]
        public void GetSyncedLibraryDataSet_SortByName_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveIdA = Guid.NewGuid();
            var curveIdB = Guid.NewGuid();
            var curveIdC = Guid.NewGuid();
            var curveA = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Name = "A",
            };
            var curveAClone = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Name = "A",
            };
            var curveB = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
            };
            var curveBClone = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
            };
            var curveC = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Name = "C",
            };
            var curveCClone = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Name = "C",
            };
            var curves = new List<PerformanceCurveDTO> { curveA, curveC, curveB };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                AddedRows = curves,
            };
            var pagingRequest = new PagingRequestModel<PerformanceCurveDTO>
            {
                SyncModel = request,
                sortColumn = "name",
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());

            var dataset = service.GetLibraryPage(libraryId, pagingRequest);

            var returnedCurves = dataset.Items;
            Assert.Equal(3, returnedCurves.Count);
            ObjectAssertions.Equivalent(curveAClone, returnedCurves[0]);
            ObjectAssertions.Equivalent(curveBClone, returnedCurves[1]);
            ObjectAssertions.Equivalent(curveCClone, returnedCurves[2]);
        }

        [Fact]
        public void GetSyncedLibraryDataSet_SortByNameDescending_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveIdA = Guid.NewGuid();
            var curveIdB = Guid.NewGuid();
            var curveIdC = Guid.NewGuid();
            var curveA = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Name = "A",
            };
            var curveAClone = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Name = "A",
            };
            var curveB = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
            };
            var curveBClone = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
            };
            var curveC = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Name = "C",
            };
            var curveCClone = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Name = "C",
            };
            var curves = new List<PerformanceCurveDTO> { curveA, curveC, curveB };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                AddedRows = curves,
            };
            var pagingRequest = new PagingRequestModel<PerformanceCurveDTO>
            {
                SyncModel = request,
                sortColumn = "name",
                isDescending = true,
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());

            var dataset = service.GetLibraryPage(libraryId, pagingRequest);

            var returnedCurves = dataset.Items;
            Assert.Equal(3, returnedCurves.Count);
            ObjectAssertions.Equivalent(curveCClone, returnedCurves[0]);
            ObjectAssertions.Equivalent(curveBClone, returnedCurves[1]);
            ObjectAssertions.Equivalent(curveAClone, returnedCurves[2]);
        }

        [Fact]
        public void GetSyncedLibraryDataSet_SortByAttribute_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveIdA = Guid.NewGuid();
            var curveIdB = Guid.NewGuid();
            var curveIdC = Guid.NewGuid();
            var curveA = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Attribute = "A",
            };
            var curveAClone = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Attribute = "A",
            };
            var curveB = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Attribute = "b",
            };
            var curveBClone = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Attribute = "b",
            };
            var curveC = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Attribute = "C",
            };
            var curveCClone = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Attribute = "C",
            };
            var curves = new List<PerformanceCurveDTO> { curveA, curveC, curveB };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                AddedRows = curves,
            };
            var pagingRequest = new PagingRequestModel<PerformanceCurveDTO>
            {
                SyncModel = request,
                sortColumn = "attribute",
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());

            var dataset = service.GetLibraryPage(libraryId, pagingRequest);

            var returnedCurves = dataset.Items;
            Assert.Equal(3, returnedCurves.Count);
            ObjectAssertions.Equivalent(curveAClone, returnedCurves[0]);
            ObjectAssertions.Equivalent(curveBClone, returnedCurves[1]);
            ObjectAssertions.Equivalent(curveCClone, returnedCurves[2]);
        }

        [Fact]
        public void GetSyncedLibraryDataSet_SortByAttributeDescending_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveIdA = Guid.NewGuid();
            var curveIdB = Guid.NewGuid();
            var curveIdC = Guid.NewGuid();
            var curveA = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Attribute = "A",
            };
            var curveAClone = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Attribute = "A",
            };
            var curveB = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Attribute = "b",
            };
            var curveBClone = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Attribute = "b",
            };
            var curveC = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Attribute = "C",
            };
            var curveCClone = new PerformanceCurveDTO
            {
                Id = curveIdC,
                Attribute = "C",
            };
            var curves = new List<PerformanceCurveDTO> { curveA, curveC, curveB };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
                AddedRows = curves,
            };
            var pagingRequest = new PagingRequestModel<PerformanceCurveDTO>
            {
                SyncModel = request,
                sortColumn = "attribute",
                isDescending = true,
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());

            var dataset = service.GetLibraryPage(libraryId, pagingRequest);

            var returnedCurves = dataset.Items;
            Assert.Equal(3, returnedCurves.Count);
            ObjectAssertions.Equivalent(curveCClone, returnedCurves[0]);
            ObjectAssertions.Equivalent(curveBClone, returnedCurves[1]);
            ObjectAssertions.Equivalent(curveAClone, returnedCurves[2]);
        }

        [Fact]
        public void GetSyncedLibraryDataSet_Search_SearchesName()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesPagingService(unitOfWork.Object);
            var libraryId = Guid.NewGuid();
            var curveIdA = Guid.NewGuid();
            var curveIdB = Guid.NewGuid();
            var curveA = new PerformanceCurveDTO
            {
                Id = curveIdA,
                Name = "A",
                Attribute = "",
                Equation = new EquationDTO(),
                CriterionLibrary = new CriterionLibraryDTO(),
            };
            var curveB = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
                Attribute = "",
                Equation = new EquationDTO(),
                CriterionLibrary = new CriterionLibraryDTO(),
            };
            var curveBClone = new PerformanceCurveDTO
            {
                Id = curveIdB,
                Name = "b",
                Attribute = "",
                Equation = new EquationDTO(),
                CriterionLibrary = new CriterionLibraryDTO() 
            };
            var curves = new List<PerformanceCurveDTO> { curveA, curveB };
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
            };
            var pagingRequest = new PagingRequestModel<PerformanceCurveDTO>
            {
                SyncModel = request,
                search = "B",
            };
            repository.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(curves);

            var dataset = service.GetLibraryPage(libraryId, pagingRequest);

            var returnedCurves = dataset.Items;
            var returnedCurve = returnedCurves.Single();
            ObjectAssertions.Equivalent(curveBClone, returnedCurve);
        }
    }
}
