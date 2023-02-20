using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class AnnouncementAuthorizationTests
    {
        [Fact]
        public async Task UserIsViewAnnouncementAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, "ViewAnnouncementClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyAnnouncementAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ModifyAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementModifyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, "ModifyAnnouncementClaim");
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewAnnouncementAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, "ViewAnnouncementClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
