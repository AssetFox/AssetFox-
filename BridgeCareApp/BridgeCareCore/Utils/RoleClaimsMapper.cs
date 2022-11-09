using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using BridgeCareCore.Security;
using BridgeCareCore.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeCareCore.Utils
{
    public class RoleClaimsMapper : IRoleClaimsMapper
    {        
        public List<string> GetInternalRoles(string securityType, List<string> IPRoles)
        {
            var internalRoles = new List<string>();
            var rolesClaimsToken = GetRolesClaimsToken(securityType);
            
            foreach (var IPRole in IPRoles)
            {
                var roleClaimsToken = rolesClaimsToken.FirstOrDefault(s => s.SelectToken("IPRoles").Children().Contains(IPRole) == true);
                if (roleClaimsToken != null)
                {
                    internalRoles.Add(roleClaimsToken.SelectToken("InternalRole").ToString());                    
                }
                else
                {
                    // TODO: Do we want to add any more IP roles values in mapping json?
                    // TODO: Should we not throw exception here? ex. we have not listed all roles that comes via idToken/Role value in request
                    // throw new UnauthorizedAccessException("Unauthorized: Invalid role present in request.");
                }
            }
            return internalRoles;
        }

        public List<string> GetClaims(string securityType, List<string> internalRoles)
        {
            var claims = new List<string>();
            var rolesClaimsToken = GetRolesClaimsToken(securityType);
            foreach (var internalRole in internalRoles)
            {
                var roleClaimsToken = rolesClaimsToken.FirstOrDefault(s => s.SelectToken("InternalRole").ToString() == internalRole);
                var claimsToken = roleClaimsToken?.SelectToken("Claims").ToString();
                claims.AddRange(JsonConvert.DeserializeObject<List<string>>(claimsToken));
            }
            return claims;
        }
        
        private static JToken GetRolesClaimsToken(string securityType)
        {            
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "rolesToClaimsMapping.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }
                        
            var content = File.ReadAllText(filePath);
            var jObject = JObject.Parse(content);
            var securityTypeToken = jObject.SelectToken("SecurityTypes").FirstOrDefault(s => s.SelectToken("SecurityType")?.ToString() == securityType);
            if(securityTypeToken == null)
            {
                throw new UnauthorizedAccessException("Unauthorized: Invalid security type present in request.");
            }
            var rolesToken = securityTypeToken.SelectToken("RolesClaims");
            return rolesToken;
        }

        public ClaimsIdentity AddClaimsToUserIdentity(ClaimsPrincipal claimsPrincipal, List<string> internalRolesFromMapper, List<string> claimsFromMapper)
        {
            var roleClaims = new List<Claim>();
            foreach (var role in internalRolesFromMapper)
            {                
                var roleClaim = new Claim(ClaimTypes.Role, role);
                if (!roleClaims.Contains(roleClaim))
                {
                    roleClaims.Add(roleClaim);
                }
            }
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(roleClaims);

            claimsFromMapper.ForEach(claim =>
            {
                if (!claimsPrincipal.HasClaim(pclaim => pclaim.Value == claim))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claim));
                }
            });
            return claimsIdentity;
        }

        public bool HasAdminAccess(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(claim => claim.Value == SecurityConstants.Claim.AdminAccess);
        }

        public bool HasSimulationAccess(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(claim => claim.Value == SecurityConstants.Claim.SimulationAccess);
        }
    }
}
