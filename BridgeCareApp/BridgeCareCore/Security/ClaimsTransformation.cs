﻿using System.Threading.Tasks;
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

        public ClaimsTransformation(IConfiguration config, IRoleClaimsMapper roleClaimsMapper, IHttpContextAccessor contextAccessor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
            
        }
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // wjwjwj _config here is a chained provider with nine children; the first child is apsettings.json
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
                var internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, new List<string>
                { SecurityConstants.Role.Administrator });
                var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.B2C, internalRolesFromMapper);
                principal.AddIdentity(_roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper));

            }

            return Task.FromResult(principal);
        }
    }
}
