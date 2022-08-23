using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BridgeCareCore.Security
{
    public class ClaimAuthorizeAttribute : TypeFilterAttribute
    {
        // Defaulting claimType = ClaimTypes.Name, if Role to be used it need to be sent as parameter 
        public ClaimAuthorizeAttribute(string claimValue, string claimType = ClaimTypes.Name) : base(typeof(ClaimAuthorizeFilter))
        {
            Arguments = new object[] { claimValue, claimType };
        }
    }

    public class ClaimAuthorizeFilter : IAuthorizationFilter
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
                       
        public ClaimAuthorizeFilter(string claimValue, string claimType)
        {
            ClaimValue = claimValue;
            ClaimType = claimType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return;
            }

            var isAuthorized = user.Claims.Any(c => c.Type == ClaimType && c.Value == ClaimValue);
            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }     
}
