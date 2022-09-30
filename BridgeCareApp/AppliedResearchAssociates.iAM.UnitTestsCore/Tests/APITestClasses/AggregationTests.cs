using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Logging;
using BridgeCareCore.Services.Aggregation;
using BridgeCareCore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework; //using AppliedResearchAssociates.iAM.Aggregation;
using Xunit;



namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class AggregationTests
    {
        private Mock<ILog> _mockLog;
        private Mock<IAggregationService> _mockService;

        public AggregationTests()
        {
            _mockLog = new Mock<ILog>();
            _mockService = new Mock<IAggregationService>();
        }

        public AggregationController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimName in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimName);
                claims.Add(claim);
            }
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new AggregationController(_mockLog.Object, _mockService.Object, EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork, hubService, accessor);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        [Fact]
        public async Task UserIsAggregateNetworkDataAuthorized()
        {
            // Admin test authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestAggregatePolicy",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAggregateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "TestAggregatePolicy");
            // Assert
            Xunit.Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsAggregateNetworkDataAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestAggregatePolicy",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAggregateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "TestAggregatePolicy");
            // Assert
            Xunit.Assert.True(allowed.Succeeded);
        }
    }
}
