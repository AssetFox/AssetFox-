﻿using System;
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
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class CalculatedAttributesTests
   {
        private static TestHelper _testHelper => TestHelper.Instance;

        private CalculatedAttributesController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new CalculatedAttributesController(_testHelper.MockEsecSecurityDBE.Object,
                _testHelper.UnitOfWork, _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        [Fact]
        public async Task UserIsModifyCalculatedAttributesFromScenarioAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.ReadOnly));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCalculatedAttributesFromScenario);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyCalculatedAttributesFromLibraryAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCalculatedAttributesFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
