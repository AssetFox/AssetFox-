using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class NetworkTests
    {
        private NetworkController CreateTestController(List<string> userClaims)
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
            var controller = new NetworkController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork, hubService, accessor);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        [Fact]
        public async Task UserIsNetworkViewAuthorized()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("NetworkViewClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "NetworkViewClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsNetworkAddAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("NetworkAddClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAddAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "NetworkAddClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsAggregateNetworkAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("NetworkAggregateClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAggregateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "NetworkAggregateClaim");
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsNetworkViewAuthorized_B2C()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("NetworkViewClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "NetworkViewClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
