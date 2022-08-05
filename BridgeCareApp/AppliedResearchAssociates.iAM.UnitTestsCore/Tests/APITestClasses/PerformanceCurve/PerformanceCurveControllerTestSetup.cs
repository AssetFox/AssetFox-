﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Security.Interfaces;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveControllerTestSetup
    {

        public static PerformanceCurveController SetupController(TestHelper testHelper, Moq.Mock<IEsecSecurity> mockedEsecSecurity)
        {
            var controller = new PerformanceCurveController(
                mockedEsecSecurity.Object,
                testHelper.UnitOfWork,
                testHelper.MockHubService.Object,
                testHelper.MockHttpContextAccessor.Object,
                TestUtils.TestServices.PerformanceCurves);
            return controller;
        }
    }
}