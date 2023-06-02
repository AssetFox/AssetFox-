using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using BridgeCareCore.Utils.Interfaces;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace BridgeCareCore.Security
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _config;
        private readonly IRoleClaimsMapper _roleClaimsMapper;
        private readonly GraphApiClientService _graphApiClientService; // TODO we can create interface and use it, register dependency in Security.cs

        public ClaimsTransformation(IConfiguration config, IRoleClaimsMapper roleClaimsMapper, IHttpContextAccessor contextAccessor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
            _graphApiClientService = new GraphApiClientService(config);
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
                principal.AddIdentity(_roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper));
            }

            if (_config.GetSection("SecurityType").Value == SecurityConstants.SecurityTypes.B2C)
            {
                // Read group name(s) set in Azure B2C(that will be IP role name: then retrieve internal role and then claims as below
                var groupNames = new List<string>();
                var groupClaimType = "group";
                if (!principal.HasClaim(claim => claim.Type == groupClaimType))
                {
                    var nameidentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                    var nameidentifier = principal.Claims.FirstOrDefault(t => t.Type == nameidentifierClaimType);

                    groupNames = await _graphApiClientService.GetGraphApiUserMemberGroup(nameidentifier.Value);
                }
                var internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, groupNames);
                
                // TODO new List<string>{ SecurityConstants.Role.Administrator } will get replaced by role found in group above
                internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, new List<string>
                 { SecurityConstants.Role.Administrator });
                var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.B2C, internalRolesFromMapper);
                principal.AddIdentity(_roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper));

            }

            return principal;
        }
    }
}
