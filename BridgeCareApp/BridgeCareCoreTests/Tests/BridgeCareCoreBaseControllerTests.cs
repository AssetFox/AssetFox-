using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers.BaseController;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class BridgeCareCoreBaseControllerTests
    {
        private BridgeCareCoreBaseController CreateController(
            IHttpContextAccessor accessor)
        {
            var security = EsecSecurityMocks.Dbe;
            var unitOfWork = TestHelper.UnitOfWork;
            var hubService = HubServiceMocks.Default();
            var controller = new BridgeCareCoreBaseController(
                security,
                unitOfWork,
                hubService,
                accessor
                );
            return controller;
        }

        [Fact]
        public void RequestHasBearer_DefaultController_True()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var controller = CreateController(accessor);
            var hasBearer = controller.RequestHasBearer();
            Assert.True(hasBearer);
        }

        [Fact]
        public void RequestHasBearer_PathIsInPathsToIgnore_False()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var context = accessor.HttpContext;
            var request = context.Request;
            var path = new PathString("/UserTokens");
            request.Path = path;
            var controller = CreateController(accessor);
            var hasBearer = controller.RequestHasBearer();
            Assert.False(hasBearer);
        }

        [Fact]
        public void RequestHasBearer_NoContextAccessor_False()
        {
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns(null as HttpContext);
            var controller = CreateController(accessor.Object);
            var hasBearer = controller.RequestHasBearer();
            Assert.False(hasBearer);
        }

        [Fact]
        public void RequestHasBearer_NoRequestInHttpContext_False()
        {
            var context = new Mock<HttpContext>();
            context.Setup(c => c.Request).Returns(null as HttpRequest);
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns(context.Object);
            var controller = CreateController(accessor.Object);
            var hasBearer = controller.RequestHasBearer();
            Assert.False(hasBearer);
        }
    }
}
