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

        public void AddClaimsPrincipalIdentities(string securityType, ClaimsPrincipal claimsPrincipal)
        {
            // Obtain the role from the claims principal
            // then parse it and pass to the internal role mapper
            var roleClaim = claimsPrincipal.Claims
                    .Single(_ => _.Type == ClaimTypes.Role).Value;
            var roleParsed = SecurityFunctions.ParseLdap(roleClaim).FirstOrDefault();

            //// TODO: Throw exception if null
            ////throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");
            var internalRoleFromMapper =GetInternalRole(SecurityConstants.SecurityTypes.Esec, roleParsed);
            var claimsFromMapper = GetClaims(SecurityConstants.SecurityTypes.Esec, internalRoleFromMapper);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsFromMapper.ForEach(claim =>
            {
                if (!claimsPrincipal.HasClaim(pclaim => pclaim.Value == claim))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claim));
                }
            });
            claimsPrincipal.AddIdentity(claimsIdentity);
        }
        public void AddClaimsToUserIdentity(HttpContext httpContext, string internalRoleFromMapper, List<string> claimsFromMapper)
        {
            var claims = new List<Claim>();
            var roleClaim = new Claim(ClaimTypes.Role, internalRoleFromMapper.ToString());
            var roleClaims = new List<Claim>
                {
                    roleClaim
                };

            // Convert the claim to a system claim for identity purposes
            claimsFromMapper.ForEach(claim =>
            {
                claims.Add(new Claim(ClaimTypes.Name, claim));
            });

            // Build the identity, add to user (claimsPrincipal)
            var identity = new ClaimsIdentity(claims);
            var roleClaimIdentity = new ClaimsIdentity(roleClaims);
            httpContext.User.AddIdentity(identity);
            httpContext.User.AddIdentity(roleClaimIdentity);

        }
    }
}
