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
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public class ClaimHelperTests
    {
        private ClaimHelper _claimHelper;
        private Guid ownerId = Guid.NewGuid();
        private Guid userId = Guid.NewGuid();
        private ISimulationQueueService _simulationQueueService = new Mock<ISimulationQueueService>().Object;

        [Fact]
        public void ShouldReturnFalseRequirePermittedCheck()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));
            var result = _claimHelper.RequirePermittedCheck();

            // Assert
            Assert.IsType<bool>(result);
            Assert.False(result);
        }        

        [Fact]
        public void ShouldReturnTrueRequirePermittedCheck()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityAddPermittedFromLibraryAccess) };

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));
            var result = _claimHelper.RequirePermittedCheck();

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }


        [Fact]
        public void ShouldPassCheckUserSimulationReadAuthorization()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.SimulationAccess) };

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));
            _claimHelper.CheckUserSimulationReadAuthorization(Guid.NewGuid(), userId, true);
        }        

        [Fact]
        public void ShouldPassCheckUserSimulationModifyAuthorization()
        {
            var claims = SystemSecurityClaimLists.Admin();

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));
            _claimHelper.CheckUserSimulationModifyAuthorization(Guid.NewGuid(), userId, false);
        }

        [Fact]
        public void ShouldExceptionOutCheckUserLibraryModifyAuthorization()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));

            var ex = Assert.Throws<UnauthorizedAccessException>(() => _claimHelper.CheckIfAdminOrOwner(ownerId, userId));
            Assert.Equal("You are not authorized to modify this library's data.", ex.Message);
        }

        [Fact]
        public void ShouldPassCheckUserLibraryModifyAuthorization()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };

            // Act
            _claimHelper = new ClaimHelper(TestHelper.UnitOfWork, _simulationQueueService, HttpContextAccessorMocks.WithClaims(claims));
            userId = ownerId;
            _claimHelper.CheckIfAdminOrOwner(ownerId, userId);
            
        }
    }
}
