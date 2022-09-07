using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils.Interfaces;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveControllerTestSetup
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public static PerformanceCurveController SetupController(TestHelper testHelper, Moq.Mock<IEsecSecurity> mockedEsecSecurity)
        {
            var controller = new PerformanceCurveController(
                mockedEsecSecurity.Object,
                testHelper.UnitOfWork,
                testHelper.MockHubService.Object,
                testHelper.MockHttpContextAccessor.Object,
                TestUtils.TestServices.PerformanceCurves,
                _mockClaimHelper.Object);
            return controller;
        }
    }
}
