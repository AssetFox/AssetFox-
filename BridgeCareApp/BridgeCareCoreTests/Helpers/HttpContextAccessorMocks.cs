using System.Collections.Generic;
using System.Security.Claims;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests
{
    public static class HttpContextAccessorMocks
    {
        private static Mock<IHttpContextAccessor> DefaultMock()
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

        public static IHttpContextAccessor WithClaims(List<Claim> claims)
        {
            var mock = mockWithClaims(claims);
            return mock.Object;

            static Mock<IHttpContextAccessor> mockWithClaims(List<Claim> claims)
            {
                var mock = new Mock<IHttpContextAccessor>();
                var context = new DefaultHttpContext();
                HttpContextSetup.AddAuthorizationHeader(context);

                var identity = new ClaimsIdentity(claims);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                context.User = claimsPrincipal;

                mock.Setup(_ => _.HttpContext).Returns(context);
                return mock;
            }
        }
    }
}
