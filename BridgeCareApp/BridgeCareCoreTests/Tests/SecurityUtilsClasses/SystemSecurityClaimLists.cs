using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCareCoreTests.Tests.SecurityUtilsClasses
{
    public static class SystemSecurityClaimLists
    {
        public static List<Claim> Admin()
        {
            var claim = SystemSecurityClaims.Admin();
            var list = new List<Claim> { claim };
            return list;
        }

        public static List<Claim> Empty()
        {
            var list = new List<Claim>();
            return list;
        }
    }
}
