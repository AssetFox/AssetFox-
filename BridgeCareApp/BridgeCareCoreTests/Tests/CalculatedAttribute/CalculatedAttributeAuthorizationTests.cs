using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class CalculatedAttributeAuthorizationTests
    {
        [Fact]
        public async Task UserIsModifyCalculatedAttributesFromScenarioAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyCalculatedAttributesFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesChangeInScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesModifyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyCalculatedAttributesFromScenario);
            // Assert
            Assert.False(allowed.Succeeded);
        }

        [Fact]
        public async Task UserIsModifyCalculatedAttributesFromLibraryAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyCalculatedAttributesFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesModifyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesChangeDefaultLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyCalculatedAttributesFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }

        [Fact]
        public async Task UserIsModifyCalculatedAttributesFromScenarioAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyCalculatedAttributesFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesChangeInScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CalculatedAttributesModifyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var claims = roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });
            var user = ClaimsPrincipals.WithNameClaims(claims);
            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyCalculatedAttributesFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
