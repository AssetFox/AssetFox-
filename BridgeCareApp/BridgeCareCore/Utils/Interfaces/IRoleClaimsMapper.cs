using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IRoleClaimsMapper
    {
        string GetInternalRole(string securityType, string IPRole);

        List<string> GetClaims(string securityType, string internalRole);

        ClaimsIdentity AddClaimsToUserIdentity(ClaimsPrincipal claimsPrincipal, string internalRoleFromMapper, List<string> claimsFromMapper);
    }
}
