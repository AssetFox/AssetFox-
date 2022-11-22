using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public static class TreatmentControllerSetup
    {
        public static TreatmentController CreateAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<ITreatmentService> treatmentServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Admin();
            var controller = CreateController(unitOfWork, claims, new Mock<IHubService>(), treatmentServiceMock);
            return controller;
        }

        public static TreatmentController CreateNonAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<ITreatmentService> treatmentServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Empty();
            var controller = CreateController(unitOfWork, claims, new Mock<IHubService>(), treatmentServiceMock);
            return controller;
        }
        public static TreatmentController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            List<Claim> contextAccessorClaims,
            Mock<IHubService> hubServiceMock = null,
            Mock<ITreatmentService> treatmentServiceMock = null
            )
        {
            var accessor = HttpContextAccessorMocks.WithClaims(contextAccessorClaims);
            return CreateController(unitOfWork, accessor, new Mock<IHubService>(), treatmentServiceMock);
        }
        public static TreatmentController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            IHttpContextAccessor contextAccessor,
            Mock<IHubService> hubServiceMock = null,
            Mock<ITreatmentService> treatmentServiceMock = null
            )
        {
            var mockedContext = new Mock<IAMContext>();
            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, mockedContext.Object);
            Mock<UnitOfDataPersistenceWork> unitOfPWork= new Mock<UnitOfDataPersistenceWork>();
            var resolveHubService = hubServiceMock ?? HubServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.Dbe;
            var simulationQueueService = new Mock<ISimulationQueueService>();
            var claimHelper = new ClaimHelper(unitOfWork.Object, simulationQueueService.Object, contextAccessor);
            var resolveTreatmentServiceMock = treatmentServiceMock ?? TreatmentServiceMocks.EmptyMock;
            var controller = new TreatmentController(
                resolveTreatmentServiceMock.Object,
                security,
                mockedRepo,
                resolveHubService.Object,
                contextAccessor,
                claimHelper);
            return controller;

        }
    }
}
