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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class UserCriteriaTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private UserCriteriaController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new UserCriteriaController(_testHelper.MockEsecSecurityAdmin.Object, _testHelper.UnitOfWork, _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        [Fact]
        public async Task UserIsViewUserCriteriaAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewUserCriteriaClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.UserCriteriaViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewUserCriteriaClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyUserCriteriaAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ModifyUserCriteriaClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.UserCriteriaModifyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.ReadOnly));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ModifyUserCriteriaClaim");
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewUserCriteriaAuthorized_B2C()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewUserCriteriaClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.UserCriteriaViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewUserCriteriaClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }

    }
}
