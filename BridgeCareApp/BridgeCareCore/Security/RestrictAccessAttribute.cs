using System;
using System.Linq;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Security
{
    /// <summary>
    ///     Restricts access to an API endpoint, only allowing requests with a valid access token to
    ///     be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        private readonly Func<string, bool> _validateRole;

        /// <summary>
        ///     Only users with the provided roles will be able to access the endpoint. If no roles
        ///     are listed, all authenticated users will be able to access it.
        /// </summary>
        /// <param name="roles">Permitted roles</param>
        public RestrictAccessAttribute(params string[] roles) => _validateRole = roles.Contains;

        public RestrictAccessAttribute() => _validateRole = role => true;

        protected bool IsAuthorized(ActionContext actionContext)
        {
            if (!TryGetAuthorization(actionContext.HttpContext.Request.Headers, out var accessToken))
            {
                return false;
            }

            var userInformationDictionary = AuthenticationController.GetUserInfoDictionary(accessToken);

            if (!userInformationDictionary.ContainsKey("roles"))
            {
                throw new UnauthorizedAccessException("User has no roles assigned.");
            }

            var userInformation = AuthenticationController.GetUserInformation(userInformationDictionary);

            // Some API endpoints need this user information, so it is inserted into
            // the request here before they process it
            actionContext.HttpContext.Request.Headers.Clear();

            actionContext.HttpContext.Request.Headers.Add("Role", userInformation.Role);
            actionContext.HttpContext.Request.Headers.Add("Name", userInformation.Name);
            actionContext.HttpContext.Request.Headers.Add("Email", userInformation.Email);

            return _validateRole(userInformation.Role);
        }

        /// <summary>
        ///     Attempts to get an authorization parameter from an HTTP request's headers
        /// </summary>
        /// <param name="headers">Request headers</param>
        /// <param name="authorization"></param>
        /// <returns>Returns true if successful</returns>
        private static bool TryGetAuthorization(IHeaderDictionary headers, out string authorization)
        {
            if (!headers.ContainsKey("Authorization"))
            {
                authorization = "";
                return false;
            }
            if (string.IsNullOrEmpty(headers["Authorization"]))
            {
                authorization = "";
                return false;
            }
            authorization = headers["Authorization"];
            return true;
        }
    }
}
