using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BridgeCareCore.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeCareCore.Utils
{
    public class RoleClaimsMapper : IRoleClaimsMapper
    {        
        public string GetInternalRole(string securityType, string IPRole)
        {
            var internalRole = string.Empty;

            // Parse json
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
               "Security", "rolesToClaimsMapping.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            // Move common code to private method
            var content = File.ReadAllText(filePath);
            var jObject = JObject.Parse(content);
            var securityTypeToken = jObject.SelectToken("SecurityTypes").FirstOrDefault(s => s.SelectToken("SecurityType")?.ToString() == securityType);
            var rolesToken = securityTypeToken.SelectToken("RolesClaims");
            var roleToken = rolesToken.FirstOrDefault(s => s.SelectToken("IPRoles").Children().Contains(IPRole) == true);
            internalRole = roleToken?.SelectToken("InternalRole").ToString();
          
            return internalRole;
        }
                
        public List<string> GetClaims(string securityType, string internalRole)
        {
            var claims = new List<string>();

            // Parse json
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
               "Security", "rolesToClaimsMapping.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            var content = File.ReadAllText(filePath);
            var jObject = JObject.Parse(content);
            var securityTypeToken = jObject.SelectToken("SecurityTypes").FirstOrDefault(s => s.SelectToken("SecurityType")?.ToString() == securityType);
            var rolesToken = securityTypeToken.SelectToken("RolesClaims");
            var roleToken = rolesToken.FirstOrDefault(s => s.SelectToken("InternalRole").ToString() == internalRole);
            var claimsToken = roleToken?.SelectToken("Claims").ToString();
            claims = JsonConvert.DeserializeObject<List<string>>(claimsToken);

            return claims;
        }
    }
}
