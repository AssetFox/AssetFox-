using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using BridgeCareCore.Security;
using SystemSecurityClaim = System.Security.Claims.Claim;

namespace BridgeCareCoreTests.Tests.SecurityUtilsClasses
{
    public static class SystemSecurityClaims
    {
        public static SystemSecurityClaim Admin()
        {
            var claim = new SystemSecurityClaim(ClaimTypes.Name, SecurityConstants.Claim.AdminAccess);
            return claim;
        }
    }
}
