using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCareCoreTests.Helpers
{
    public static class ClaimsPrincipals
    {
        public static ClaimsPrincipal WithClaims(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }

        public static ClaimsPrincipal WithNameClaims(List<string> claimNames)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimName in claimNames)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimName);
                claims.Add(claim);
            }
            return WithClaims(claims);
        }
    }
}
