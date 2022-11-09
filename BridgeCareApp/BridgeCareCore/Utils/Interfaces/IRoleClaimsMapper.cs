using System.Collections.Generic;
using System.Security.Claims;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IRoleClaimsMapper
    {
        List<string> GetInternalRoles(string securityType, List<string> IPRoles);

        List<string> GetClaims(string securityType, List<string> internalRole);

        ClaimsIdentity AddClaimsToUserIdentity(ClaimsPrincipal claimsPrincipal, List<string> internalRoleFromMapper, List<string> claimsFromMapper);

        bool HasAdminAccess(ClaimsPrincipal claimsPrincipal);

        bool HasSimulationAccess(ClaimsPrincipal claimsPrincipal);
    }
}
