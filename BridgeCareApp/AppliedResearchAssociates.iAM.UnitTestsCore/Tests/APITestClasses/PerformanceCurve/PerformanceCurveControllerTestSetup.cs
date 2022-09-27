using System;
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

        public static PerformanceCurveController SetupController(IEsecSecurity esecSecurity)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new PerformanceCurveController(
                esecSecurity,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                TestUtils.TestServices.PerformanceCurves(TestHelper.UnitOfWork, hubService)
                );
            return controller;
        }
    }
}
