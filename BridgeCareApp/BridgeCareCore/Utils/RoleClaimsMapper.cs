using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using BridgeCareCore.Security;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeCareCore.Utils
{
    public class RoleClaimsMapper : IRoleClaimsMapper
    {        
        public string GetInternalRole(string securityType, string IPRole)
        {
            var internalRole = string.Empty;

            var rolesClaimsToken = GetRolesClaimsToken(securityType);
            var roleClaimsToken = rolesClaimsToken.FirstOrDefault(s => s.SelectToken("IPRoles").Children().Contains(IPRole) == true);
            if(roleClaimsToken == null)
            {
                throw new UnauthorizedAccessException("Unauthorized: Invalid role present in request.");
            }
            internalRole = roleClaimsToken.SelectToken("InternalRole").ToString();

            return internalRole;
        }        

        public List<string> GetClaims(string securityType, string internalRole)
        {
            var claims = new List<string>();

            var rolesClaimsToken = GetRolesClaimsToken(securityType);
            var roleClaimsToken = rolesClaimsToken.FirstOrDefault(s => s.SelectToken("InternalRole").ToString() == internalRole);
            var claimsToken = roleClaimsToken?.SelectToken("Claims").ToString();
            claims = JsonConvert.DeserializeObject<List<string>>(claimsToken);

            return claims;
        }
        
        private static JToken GetRolesClaimsToken(string securityType)
        {            
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
               "Security", "rolesToClaimsMapping.json");

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

        public ClaimsIdentity AddClaimsToUserIdentity(ClaimsPrincipal claimsPrincipal, string internalRoleFromMapper, List<string> claimsFromMapper)
        {
            var roleClaim = new Claim(ClaimTypes.Role, internalRoleFromMapper);
            var roleClaims = new List<Claim> { roleClaim };

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
    }
}
