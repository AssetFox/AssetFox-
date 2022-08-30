using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IRoleClaimsMapper
    {
        string GetInternalRole(string securityType, string IPRole);

        List<string> GetClaims(string securityType, string internalRole);

        void AddClaimsPrincipalIdentities(string securityType, ClaimsPrincipal claimsPrincipal);

        void AddClaimsToUserIdentity(HttpContext httpContext, string internalRoleFromMapper, List<string> claimsFromMapper);
    }
}
