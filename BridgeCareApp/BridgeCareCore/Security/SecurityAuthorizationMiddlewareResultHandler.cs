using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Security
{
    public class SecurityAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden)
            {
                // Set status code to forbidden
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                string controllerName = "";
                if (context.Request.Path.Value.Split('/').Count() > 2) controllerName = context.Request.Path.Value.Split('/')[2];

                // Send response back to UI
                await context.Response.WriteAsync("User is not authorized for " + controllerName + "controller!");
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        public class Show404Requirement : IAuthorizationRequirement { }
    }
}
