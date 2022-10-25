using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Utils;
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
            Mock<IHubService> hubServiceMock = null
            )
        {
            var resolveHubService = hubServiceMock ?? new Mock<IHubService>();
            var security = EsecSecurityMocks.Dbe;
            var mockDataService = new Mock<IInvestmentDefaultDataService>();
            var claimHelper = new ClaimHelper(unitOfWork.Object, contextAccessor);
            var investmentBudgetServiceMock = InvestmentBudgetServiceMocks.New();
            var controller = new InvestmentController(
                investmentBudgetServiceMock.Object,
                security,
                unitOfWork.Object,
                resolveHubService.Object,
                contextAccessor,
                mockDataService.Object,
                claimHelper);
            return controller;

        }

        public static InvestmentController CreateController(
            Mock<IUnitOfWork> unitOfWork,
            List<Claim> contextAccessorClaims,
            Mock<IHubService> hubServiceMock = null
            )
        {
            var accessor = HttpContextAccessorMocks.WithClaims(contextAccessorClaims);
            return CreateController(unitOfWork, accessor, hubServiceMock);
        }

        public static InvestmentController CreateAdminController(Mock<IUnitOfWork> unitOfWork, Mock<IHubService> hubServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Admin();
            var controller = CreateController(unitOfWork, claims, hubServiceMock);
            return controller;
        }

        public static InvestmentController CreateNonAdminController(Mock<IUnitOfWork> unitOfWork, Mock<IHubService> hubServiceMock = null)
        {
            var claims = SystemSecurityClaimLists.Empty();
            var controller = CreateController(unitOfWork, claims, hubServiceMock);
            return controller;
        }
    }
}
