﻿using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils.Interfaces;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
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
                AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils.TestServices.PerformanceCurves(TestHelper.UnitOfWork, hubService),
                                                            _mockClaimHelper.Object);
            return controller;
        }
    }
}
