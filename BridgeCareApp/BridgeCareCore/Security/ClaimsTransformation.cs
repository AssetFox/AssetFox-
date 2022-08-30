using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using BridgeCareCore.Utils.Interfaces;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace BridgeCareCore.Security
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _config;
        private readonly IRoleClaimsMapper _roleClaimsMapper;

        public ClaimsTransformation(IConfiguration config, IRoleClaimsMapper roleClaimsMapper, IHttpContextAccessor contextAccessor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
            
        }
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (_config.GetSection("SecurityType").Value == SecurityConstants.SecurityTypes.Esec)
            {
                // Obtain the role from the claims principal
                // then parse it and pass to the internal role mapper
                var roleClaim = principal.Claims
                        .Single(_ => _.Type == ClaimTypes.Role)?.Value;
                var roleParsed = SecurityFunctions.ParseLdap(roleClaim)?.FirstOrDefault();
                if (roleParsed == null)
                {
                    throw new UnauthorizedAccessException("No role found.");
                }
                var internalRoleFromMapper = _roleClaimsMapper.GetInternalRole(SecurityConstants.SecurityTypes.Esec, roleParsed);
                var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.Esec, internalRoleFromMapper);
                principal.AddIdentity(_roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRoleFromMapper, claimsFromMapper));
            }
            if (_config.GetSection("SecurityType").Value == SecurityConstants.SecurityTypes.B2C)
            {
                var internalRoleFromMapper = _roleClaimsMapper.GetInternalRole(SecurityConstants.SecurityTypes.B2C, "Administrator");
                var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.B2C, internalRoleFromMapper);
                principal.AddIdentity(_roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRoleFromMapper, claimsFromMapper));

            }
            return Task.FromResult(principal);
        }
    }
}
