using System;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;

namespace BridgeCareCore.Security
{
    public class RestrictAccessHandler : AuthorizationHandler<UserHasAllowedRoleRequirement>
    {
        private readonly IEsecSecurity _esecSecurity;

        public RestrictAccessHandler(IEsecSecurity esecSecurity) =>
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserHasAllowedRoleRequirement requirement)
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

            return Task.CompletedTask;
        }
    }

    public class UserHasAllowedRoleRequirement : IAuthorizationRequirement
    {
        public UserHasAllowedRoleRequirement(params string[] roles) => Roles = roles;

        public string[] Roles { get; }
    }
}
