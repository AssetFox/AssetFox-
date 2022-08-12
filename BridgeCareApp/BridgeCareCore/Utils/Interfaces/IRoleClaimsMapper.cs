using System.Collections.Generic;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IRoleClaimsMapper
    {
        string GetInternalRole(string securityType, string IPRole);

        List<string> GetClaims(string securityType, string internalRole);
    }
}
