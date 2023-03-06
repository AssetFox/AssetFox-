using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Tests.PerformanceCurve;
using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests.Tests { 
    public static class PerformanceCurveControllerTestSetup
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        public static PerformanceCurveController Create(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IPerformanceCurvesService performanceCurvesService, IPerformanceCurvesPagingService performanceCurvesPagingService)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new PerformanceCurveController(
                esecSecurity,
                unitOfWork,
                hubService,
                accessor,
                performanceCurvesService,
                performanceCurvesPagingService,
                _mockClaimHelper.Object);
            return controller;
        }
        public static PerformanceCurveController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            IHttpContextAccessor contextAccessor,
            Mock<IHubService> hubServiceMock = null,
            Mock<IPerformanceCurvesService> performanceCurveServiceMock = null,
            Mock<IPerformanceCurvesPagingService> performanceCurvePagingServiceMock = null
            )
        {
            var resolveHubService = hubServiceMock ?? HubServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.Dbe;
            var mockDataService = new Mock<IInvestmentDefaultDataService>();
            var simulationQueueService = new Mock<ISimulationQueueService>();
            var claimHelper = new ClaimHelper(unitOfWork.Object, simulationQueueService.Object, contextAccessor);
            var resolvePerformanceCurveServiceMock = performanceCurveServiceMock ?? PerformanceCurveServiceMocks.New();
            var resolvePerformanceCurvePagingServiceMock = performanceCurvePagingServiceMock ?? PerformanceCurvesPagingServiceMocks.New();
            var controller = new PerformanceCurveController(
                security,
                unitOfWork.Object,
                resolveHubService.Object,
                contextAccessor,
                resolvePerformanceCurveServiceMock.Object,
                resolvePerformanceCurvePagingServiceMock.Object,
                claimHelper);
            return controller;
        }

        public static PerformanceCurveController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            List<Claim> contextAccessorClaims,
            Mock<IHubService> hubServiceMock = null,
            Mock<IPerformanceCurvesService> performanceCurveServiceMock = null
            )
        {
            var accessor = HttpContextAccessorMocks.WithClaims(contextAccessorClaims);
            return CreateController(unitOfWork, accessor, hubServiceMock, performanceCurveServiceMock);
        }
        public static PerformanceCurveController CreateNonAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<IPerformanceCurvesService> performanceCurveServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Empty();
            var controller = CreateController(unitOfWork, claims, hubServiceMock, performanceCurveServiceMock);
            return controller;
        }
        public static PerformanceCurveController CreateAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<IPerformanceCurvesService> performanceCurveServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Admin();
            var controller = CreateController(unitOfWork, claims, hubServiceMock, performanceCurveServiceMock); 
            return controller;
        }
    }
}
