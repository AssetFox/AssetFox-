using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
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
            // wjwjwj also this test
            var unitOfWork = UnitOfWorkMocks.New();
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.New();
            var repository = new Mock<IPerformanceCurveRepository>();
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repository.Object);
            var service = new PerformanceCurvesService(unitOfWork.Object, hubService, expressionValidationService.Object);
            var libraryId = Guid.NewGuid();
            var request = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
            };
            var curves = new List<PerformanceCurveDTO>();
            repository.Setup(r => r.GetPerformanceCurvesForLibrary(libraryId)).Returns(curves);

            var dataset = service.GetSyncedLibraryDataset(libraryId, request);
            Assert.Empty(dataset);
        }
    }
}
