using System.Security.Claims;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class RemainingLifeLimitAuthorizationTests
    {
        [Fact]
        public async Task UserIsViewRemainingLifeLimitFromLibraryAuthorized()
        {
            // Admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewRemainingLifeLimitFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.RemainingLifeLimitViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ViewRemainingLifeLimitFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyRemainingLifeLimitFromScenarioAuthorized()
        {
            // Non-admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyRemainingLifeLimitFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.RemainingLifeLimitModifyPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyRemainingLifeLimitFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteRemainingLifeLimitFromLibrary()
        {
            // Non-admin unauthorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteRemainingLifeLimitFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.RemainingLifeLimitDeleteAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.RemainingLifeLimitDeletePermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.DeleteRemainingLifeLimitFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewRemainingLifeLimitFromLibraryAuthorized_B2C()
        {
            // Admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewRemainingLifeLimitFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.RemainingLifeLimitViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ViewRemainingLifeLimitFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
