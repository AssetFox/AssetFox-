using System.Collections.Generic;
using System.Security.Claims;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests
{
    public static class HttpContextAccessorMocks
    {
        public static Mock<IHttpContextAccessor> DefaultMock()
        {
            var mock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(context);
            mock.Setup(_ => _.HttpContext).Returns(context);
            return mock;
        }

        public static IHttpContextAccessor Default()
        {
            var mock = DefaultMock();
            return mock.Object;
        }

        public static Mock<IHttpContextAccessor> MockWithClaims(List<Claim> claims)
        {
            var mock = new Mock<IHttpContextAccessor>();
            mock.AddClaims(claims);
            return mock;
        }

        public static void AddClaims(this Mock<IHttpContextAccessor> mock, List<Claim> claims)
        {
            var context = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(context);

            var claimsPrincipal = ClaimsPrincipals.WithClaims(claims);
            context.User = claimsPrincipal;

            mock.Setup(_ => _.HttpContext).Returns(context);
        }

        public static IHttpContextAccessor WithClaims(List<Claim> claims)
        {
            var mock = MockWithClaims(claims);
            return mock.Object;
        }
    }
}
