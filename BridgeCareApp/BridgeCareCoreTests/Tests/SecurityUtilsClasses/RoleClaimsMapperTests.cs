using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BridgeCareCore.Security;
using BridgeCareCore.Utils;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class RoleClaimsMapperTests
    {
        private RoleClaimsMapper _roleClaimsMapper;

        [Fact]
        public void ShouldReturnRoleGetInternalRole()
        {
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator });

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal(result.First(), SecurityConstants.Role.Administrator);
        }

        // TODO add edge cases for GetInternalRole later

        [Fact]
        public void ShouldReturnClaimsGetClaims()
        {
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor });

            // Assert
            Assert.IsType<List<string>>(result);
            Assert.Equal(61, result.Count);
        }

        [Fact]
        public void ShouldReturnTrueHasAdminAccess()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };
            var claimsPrincipal = MockHttpContext(claims);

            // Act
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.HasAdminAccess(claimsPrincipal);

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        [Fact]
        public void ShouldReturnFalseHasAdminAccess()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.PerformanceCurveAddPermittedFromLibraryAccess) };
            var claimsPrincipal = MockHttpContext(claims);

            // Act
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.HasAdminAccess(claimsPrincipal);

            // Assert
            Assert.IsType<bool>(result);
            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnTrueHasSimulationAccess()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.SimulationAccess) };
            var claimsPrincipal = MockHttpContext(claims);

            // Act
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.HasSimulationAccess(claimsPrincipal);

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        [Fact]
        public void ShouldReturnFalseHasSimulationAccess()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess) };
            var claimsPrincipal = MockHttpContext(claims);

            // Act
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.HasSimulationAccess(claimsPrincipal);

            // Assert
            Assert.IsType<bool>(result);
            Assert.False(result);
        }

        [Fact]
        public void ShouldPassAddClaimsToUserIdentity()
        {
            var claims = new List<string> { SecurityConstants.Claim.AdminAccess };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            _roleClaimsMapper = new RoleClaimsMapper();
            var result = _roleClaimsMapper.AddClaimsToUserIdentity(claimsPrincipal, new List<string> { "Administrator" }, claims);

            // Assert
            Assert.IsType<ClaimsIdentity>(result);
            Assert.True(result.Claims.Any(c => c.Type == ClaimTypes.Role));
            Assert.True(result.Claims.Any(c => c.Type == ClaimTypes.Name));
        }

        private ClaimsPrincipal MockHttpContext(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }
    }
}
