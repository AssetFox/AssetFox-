using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
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
    }
}
