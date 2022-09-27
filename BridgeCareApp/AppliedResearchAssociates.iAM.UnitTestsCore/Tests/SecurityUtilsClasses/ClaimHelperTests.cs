using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using Xunit;
using Moq;
using Assert = Xunit.Assert;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using BridgeCareCore.Security;
using System;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SecurityUtilsClasses
{
    public class ClaimHelperTests
    {
        ClaimHelper _claimHelper;
        private static TestHelper _testHelper => TestHelper.Instance;
        Guid ownerId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        public void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();            
        }

        [Fact]
        public void ShouldReturnFalseRequirePermittedCheck()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);
            var result = _claimHelper.RequirePermittedCheck();

            // Assert
            Assert.IsType<bool>(result);
            Assert.False(result);
        }        

        [Fact]
        public void ShouldReturnTrueRequirePermittedCheck()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityAddPermittedFromLibraryAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);
            var result = _claimHelper.RequirePermittedCheck();

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }


        [Fact]
        public void ShouldPassCheckUserSimulationReadAuthorization()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.SimulationAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);
            _claimHelper.CheckUserSimulationReadAuthorization(Guid.NewGuid(), userId, true);
        }        

        [Fact]
        public void ShouldPassCheckUserSimulationModifyAuthorization()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);
            _claimHelper.CheckUserSimulationModifyAuthorization(Guid.NewGuid(), userId, false);
        }

        [Fact]
        public void ShouldExceptionOutCheckUserLibraryModifyAuthorization()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);

            var ex = Assert.Throws<UnauthorizedAccessException>(() => _claimHelper.CheckUserLibraryModifyAuthorization(ownerId, userId));
            Assert.Equal("You are not authorized to modify this library's data.", ex.Message);
        }

        [Fact]
        public void ShouldPassCheckUserLibraryModifyAuthorization()
        {
            Setup();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.BudgetPriorityViewPermittedFromLibraryAccess) };
            MockHttpContext(claims);

            // Act
            _claimHelper = new ClaimHelper(_testHelper.UnitOfWork, _testHelper.MockHttpContextAccessor.Object);
            userId = ownerId;
            _claimHelper.CheckUserLibraryModifyAuthorization(ownerId, userId);
            
        }

        private static void MockHttpContext(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
            _testHelper.MockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        }
    }
}
