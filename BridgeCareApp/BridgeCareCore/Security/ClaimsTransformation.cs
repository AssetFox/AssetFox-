﻿using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using BridgeCareCore.Utils.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using BridgeCareCore.Security.Interfaces;
using AppliedResearchAssociates.iAM.Common;

namespace BridgeCareCore.Security
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _config;
        private readonly IRoleClaimsMapper _roleClaimsMapper;
        private readonly IGraphApiClientService _graphApiClientService;
        private Dictionary<string, UserCache> cache = new();
        private readonly ILog _log;

        public ClaimsTransformation(IConfiguration config, IRoleClaimsMapper roleClaimsMapper, IGraphApiClientService graphApiClientService, ILog log)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
            _graphApiClientService = graphApiClientService ?? throw new ArgumentNullException(nameof(graphApiClientService));
            _log = log ?? throw new ArgumentNullException(nameof(log));
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
                var nameidentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                var nameidentifier = principal.Claims.FirstOrDefault(t => t.Type == nameidentifierClaimType);
                var cacheKey = nameidentifier?.Value;
                lock (cache)
                {
                    try
                    {
                        if (cacheKey != null && !cache.ContainsKey(cacheKey) || cache[cacheKey].LastRefreshTime < DateTime.Now.AddDays(-1))
                        {
                            // Read group name(s) set in Azure B2C(that will be IP role name(s)
                            var groupNames = new List<string>();
                            var groupClaimType = "group";
                            if (!principal.HasClaim(claim => claim.Type == groupClaimType))
                            {                                
                                groupNames = _graphApiClientService.GetGraphApiUserMemberGroup(cacheKey)?.Result;
                            }
                            if (groupNames.Count == 0)
                            {
                                groupNames.Add(SecurityConstants.Role.Default);
                            }

                            var internalRolesFromMapper = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, groupNames);
                            var claimsFromMapper = _roleClaimsMapper.GetClaims(SecurityConstants.SecurityTypes.B2C, internalRolesFromMapper);
                            var identity = _roleClaimsMapper.AddClaimsToUserIdentity(principal, internalRolesFromMapper, claimsFromMapper);

                            if (!cache.ContainsKey(cacheKey))
                            {
                                cache.Add(cacheKey, new UserCache { Identity = identity, LastRefreshTime = DateTime.Now });
                            }
                            else
                            {
                                if (cache[cacheKey].LastRefreshTime < DateTime.Now.AddDays(-1))
                                {
                                    cache[cacheKey].Identity = identity;
                                    cache[cacheKey].LastRefreshTime = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Ignore: control gets here few times due to any concurrent calls. Solution to avoid any exception can be looked at later.
                        _log.Error("ClaimsTransformation Error: " + e.StackTrace);
                    }
                }
                if (cacheKey != null)
                {
                    principal.AddIdentity(cache[cacheKey].Identity);
                }
            }
            return principal;
        }
    }    
}
