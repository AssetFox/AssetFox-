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
            // If the authorization was forbidden and the resource had a specific requirement,
            // provide a custom 404 response.
            //if (authorizeResult.Forbidden
            //    && authorizeResult.AuthorizationFailure!.FailedRequirements
            //        .OfType<Show404Requirement>().Any())
            //{
            //    // Return a 404 to make it appear as if the resource doesn't exist.
            //    context.Response.StatusCode = StatusCodes.Status404NotFound;
            //    return;
            //}
            if (authorizeResult.Forbidden)
            {
                //HubService.SendRealTimeMessage("test",HubConstant.BroadcastError, $"Authorization Forbidden: {authorizeResult.ToString}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //var missingClaims = authorizeResult.AuthorizationFailure.FailedRequirements.FirstOrDefault().ToString();
                await context.Response.WriteAsync("User is not authorized for this page!");
                //await context.Response.WriteAsync(missingClaims.Split('(')[1] + " are not authorized for this user!");
                var bdy = context.Response.Headers;
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        public class Show404Requirement : IAuthorizationRequirement { }
    }
}
