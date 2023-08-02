using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using Xunit;
using Assert = Xunit.Assert;
using System.Security.Claims;
using System.Collections.Generic;
using BridgeCareCore.Security;
using System;
using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
using BridgeCareCore.Interfaces;
using Moq;using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Http;
using BridgeCareCoreTests.Helpers;

namespace BridgeCareCoreTests.Tests
{
    public class ClaimHelperTests
    {
        private ClaimHelper CreateClaimHelper(
            Mock<IHttpContextAccessor> contextAccessor,
            Mock<IUnitOfWork> unitOfWork = null)
        {
            var simulationQueueService = new Mock<IWorkQueueService>();
            var resolveUnitOfWork = unitOfWork ?? UnitOfWorkMocks.New();
            var claimHelper = new ClaimHelper(
                resolveUnitOfWork.Object,
                contextAccessor.Object);
            return claimHelper;
        }

        private IWorkQueueService _simulationQueueService = new Mock<IWorkQueueService>().Object;

        [Fact]
        public void RequirePermittedCheck_AdminAccess_False()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);
            // Act

            var result = claimHelper.RequirePermittedCheck();

            Assert.False(result);
        }        

        [Fact]
        public void RequirePermittedCheck_NoAdminAccess_True()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityAddPermittedFromLibraryAccess) };
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);

            // Act
            var result = claimHelper.RequirePermittedCheck();

            Assert.True(result);
        }

        [Fact]
        public void CheckUserSimulationReadAuthorization_SimulationAccess_Passes()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.SimulationAccess) };
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);
            var userId = Guid.NewGuid();

            // Act
            claimHelper.CheckUserSimulationReadAuthorization(Guid.NewGuid(), userId, true);
        }        

        [Fact]
        public void CheckUserSimulationModifyAuthorization_AdminUser_Passes()
        {
            var claims = SystemSecurityClaimLists.Admin();
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);
            var userId = Guid.NewGuid();
            // Act
            claimHelper.CheckUserSimulationModifyAuthorization(Guid.NewGuid(), userId, false);
        }

        [Fact]
        public void OldWayCheckUserLibraryModifyAuthorization_NotAuthorized_Throws()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);
            var ownerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            var ex = Assert.Throws<UnauthorizedAccessException>(() => claimHelper.CheckIfAdminOrOwner(ownerId, userId));
        }

        [Fact]
        public void OldWayCheckUserLibraryModifyAuthorization_UserIsOwner_Passes()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };
            var contextAccessor = HttpContextAccessorMocks.MockWithClaims(claims);
            var claimHelper = CreateClaimHelper(contextAccessor);
            var ownerId = Guid.NewGuid();

            // Act
            claimHelper.CheckIfAdminOrOwner(ownerId, ownerId);
        }
    }
}
