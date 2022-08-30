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
            _roleClaimsMapper.AddClaimsPrincipalIdentities(_config.GetSection("SecurityType").Value, principal);

            return Task.FromResult(principal);
        }
    }
}
