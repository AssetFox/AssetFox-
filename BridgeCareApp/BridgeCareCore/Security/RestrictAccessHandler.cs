using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace BridgeCareCore.Security
{
    public class RestrictAccessHandler : AuthorizationHandler<UserHasAllowedRoleRequirement>
    {
        private IConfiguration _config;

        public RestrictAccessHandler(IConfiguration config) =>
            _config = config ?? throw new ArgumentNullException(nameof(config));

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserHasAllowedRoleRequirement requirement)
        {
            var security = _config.GetSection("Security").Value;

            if (security == SecurityConstants.Security.Esec)
            {
                if (!context.User.HasClaim(_ => _.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"))
                {
                    return Task.CompletedTask;
                }

                var roleClaim = context.User.Claims
                    .Single(_ => _.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                requirement.Roles.ForEach(role =>
                {
                    if (roleClaim.Contains(role))
                    {
                        context.Succeed(requirement);
                    }
                });
            }

            if (security == SecurityConstants.Security.B2C)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class UserHasAllowedRoleRequirement : IAuthorizationRequirement
    {
        public UserHasAllowedRoleRequirement(params string[] roles) => Roles = roles;

        public string[] Roles { get; }
    }
}
