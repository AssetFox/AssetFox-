using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using BridgeCareCore.Utils.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using BridgeCareCore.Security.Interfaces;

namespace BridgeCareCore.Security
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _config;
        private readonly IRoleClaimsMapper _roleClaimsMapper;
        private readonly IGraphApiClientService _graphApiClientService;
        private Dictionary<string, UserCache> cache = new();

        public ClaimsTransformation(IConfiguration config, IRoleClaimsMapper roleClaimsMapper, IGraphApiClientService graphApiClientService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
            _graphApiClientService = graphApiClientService ?? throw new ArgumentNullException(nameof(graphApiClientService));
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (_config.GetSection("SecurityType").Value == SecurityConstants.SecurityTypes.Esec)
            {
                // Obtain the role(s) from the claims principal
                var roleClaim = principal.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role).Value;
                var rolesParsed = new List<string>();
                if (roleClaim != null)
                {
                    rolesParsed = SecurityFunctions.ParseLdap(roleClaim);
                }
                if (rolesParsed.Count == 0)
                {
                    rolesParsed.Add(SecurityConstants.Role.Default);
                }

                var internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.Esec, rolesParsed);
                var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.Esec, internalRolesFromMapper);
                var identity = _roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper);
                principal.AddIdentity(identity);
            }

            if (_config.GetSection("SecurityType").Value == SecurityConstants.SecurityTypes.B2C)
            {
                // Read user name
                var userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
                var userName = principal.Claims.FirstOrDefault(t => t.Type == userNameClaimType);
                var userNameValue = userName?.Value;
                try
                {
                    if (!cache.ContainsKey(userNameValue) || cache[userNameValue].LastRefreshTime < DateTime.Now.AddDays(-1))
                    {
                        // Read group name(s) set in Azure B2C(that will be IP role name(s)
                        var groupNames = new List<string>();
                        var groupClaimType = "group";
                        if (!principal.HasClaim(claim => claim.Type == groupClaimType))
                        {
                            var nameidentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                            var nameidentifier = principal.Claims.FirstOrDefault(t => t.Type == nameidentifierClaimType);
                            groupNames = await _graphApiClientService.GetGraphApiUserMemberGroup(nameidentifier.Value);
                        }
                        if (groupNames.Count == 0)
                        {
                            groupNames.Add(SecurityConstants.Role.Default);
                        }

                        var internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, groupNames);
                        var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.B2C, internalRolesFromMapper);
                        var identity = _roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper);

                        if (!cache.ContainsKey(userNameValue))
                        {
                            cache.Add(userNameValue, new UserCache { Identity = identity, LastRefreshTime = DateTime.Now });
                        }
                        else
                        {
                            if (cache[userNameValue].LastRefreshTime < DateTime.Now.AddDays(-1))
                            {
                                cache[userNameValue].Identity = identity;
                                cache[userNameValue].LastRefreshTime = DateTime.Now;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ignore: gets here due to concurrent calls
                    var err = ex.Message;
                }
                principal.AddIdentity(cache[userNameValue].Identity);
            }
            return principal;
        }
    }    
}
