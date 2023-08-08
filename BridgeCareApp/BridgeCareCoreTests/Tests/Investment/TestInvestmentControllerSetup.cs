using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Tests.General_Work_Queue;
using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class TestInvestmentControllerSetup
    {

        public static InvestmentController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            IHttpContextAccessor contextAccessor,
            Mock<IHubService> hubServiceMock = null,
            Mock<IInvestmentBudgetsService> investmentBudgetServiceMock = null,
            Mock<IInvestmentPagingService> investmentPagingServiceMock = null
            )
        {
            var resolveHubService = hubServiceMock ?? HubServiceMocks.DefaultMock();
            var security = EsecSecurityMocks.Dbe;
            var mockDataService = new Mock<IInvestmentDefaultDataService>();
            var simulationQueueService = new Mock<IWorkQueueService>();
            var claimHelper = new ClaimHelper(unitOfWork.Object, contextAccessor);
            var resolveInvestmentBudgetServiceMock = investmentBudgetServiceMock ?? InvestmentBudgetServiceMocks.New();
            var resolveInvestmentPagingServiceMock = investmentPagingServiceMock ?? new Mock<IInvestmentPagingService>();
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            var controller = new InvestmentController(
                resolveInvestmentBudgetServiceMock.Object,
                resolveInvestmentPagingServiceMock.Object,
                security,
                unitOfWork.Object,
                resolveHubService.Object,
                contextAccessor,
                mockDataService.Object,
                claimHelper,
                generalWorkQueue.Object);
            return controller;

        }

        public static InvestmentController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            List<Claim> contextAccessorClaims,
            Mock<IHubService> hubServiceMock = null,
            Mock<IInvestmentBudgetsService> investmentBudgetServiceMock = null
            )
        {
            var accessor = HttpContextAccessorMocks.WithClaims(contextAccessorClaims);
            return CreateController(unitOfWork, accessor, hubServiceMock, investmentBudgetServiceMock);
        }

        public static InvestmentController CreateAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<IInvestmentBudgetsService> investmentBudgetServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Admin();
            var controller = CreateController(unitOfWork, claims, hubServiceMock, investmentBudgetServiceMock);
            return controller;
        }

        public static InvestmentController CreateNonAdminController(
            Mock<IUnitOfWork> unitOfWork,
            Mock<IHubService> hubServiceMock = null,
            Mock<IInvestmentBudgetsService> investmentBudgetServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Empty();
            var controller = CreateController(unitOfWork, claims, hubServiceMock, investmentBudgetServiceMock);
            return controller;
        }
    }
}
