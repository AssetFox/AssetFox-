using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests { 
    public static class PerformanceCurveControllerTestSetup
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public static PerformanceCurveController SetupController(IEsecSecurity esecSecurity)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new PerformanceCurveController(esecSecurity,
                                                            TestHelper.UnitOfWork,
                                                            hubService,
                                                            accessor,
                                                            TestServices.PerformanceCurves(TestHelper.UnitOfWork, hubService),
                                                            _mockClaimHelper.Object
                                                            );
            return controller;
        }

        public static PerformanceCurveController WithUnitOfWork(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var performanceCurvesService = TestServices.PerformanceCurves(unitOfWork, hubService);
            var controller = new PerformanceCurveController(
                esecSecurity,
                unitOfWork,
                hubService,
                accessor,
                performanceCurvesService,
                _mockClaimHelper.Object);
            return controller;
        }
    }
}
