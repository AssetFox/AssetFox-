using System.Linq;
using System.Threading.Tasks;
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
                string controllerName = string.Empty;
                var path = context.Request.Path;
                if (path.Value.Split('/').Count() > 2)
                {
                    controllerName = context.Request.Path.Value.Split('/')[2];
                }

                string responseText;
                // Set status code to forbidden
                if (path.Value != null && (path.Value.Contains("GetHasPermittedAccess") || path.Value.Contains("GetHasAdminAccess")))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    responseText = "false";
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    responseText = "User is not authorized for " + controllerName + "controller!";
                }

                // Send response back to UI
                await context.Response.WriteAsync(responseText);
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
