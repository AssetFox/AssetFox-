using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Hubs;
using System.Linq;
using static BridgeCareCore.Security.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using BridgeCareCore.Security;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BridgeCareCoreBaseController
    {
        public const string AuthenticationError = "Authentication Error";
        private static IConfigurationSection _esecConfig;
        private readonly ILog _log;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AuthenticationController(IConfiguration config, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, ILog log) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _esecConfig = config?.GetSection("EsecConfig") ?? throw new ArgumentNullException(nameof(config));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        ///     API endpoint for fetching user info from ESEC using the OpenID Connect protocol
        /// </summary>
        /// <param name="token">The user's access token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserInfo/{token}")]
        public IActionResult GetUserInfo(string token)
        {
            try
            {
                var response = GetUserInfoString(token);
                var responseResult = response.Result;
                ValidateResponse(responseResult);
                var userInfo = JsonConvert.DeserializeObject<UserInfoDTO>(responseResult);
                userInfo.HasAdminAccess = UserInfo.HasAdminAccess;
                userInfo.HasSimulationAccess = UserInfo.HasSimulationAccess;

                return Ok(userInfo);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AuthenticationError}::GetUserInfo - {e.Message}", e);
                return Ok();
            }
        }

        /// <summary>
        ///     Fetches user info as a JSON-formatted string from ESEC
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>JSON-formatted user info</returns>
        private static async Task<string> GetUserInfoString(string token)
        {
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler) { BaseAddress = new Uri(_esecConfig["ESECBaseAddress"]) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("access_token", WebUtility.UrlDecode(token))
            };
            HttpContent content = new FormUrlEncodedContent(formData);

            var responseTask = await client.PostAsync("userinfo", content);

            return responseTask.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        ///     API endpoint for fetching ID and Access tokens from ESEC using the OpenID Connect protocol
        /// </summary>
        /// <param name="code">The authentication or error code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserTokens/{code}")]
        public async Task<IActionResult> GetUserTokens(string code)
        {
            try
            {
                // These two lines should be removed as soon as the ESEC site's certificates start working
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler) { BaseAddress = new Uri(_esecConfig["EsecBaseAddress"]) };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", WebUtility.UrlDecode(code)),
                    new KeyValuePair<string, string>("redirect_uri", _esecConfig["EsecRedirect"]),
                    new KeyValuePair<string, string>("client_id", _esecConfig["EsecClientId"]),
                    new KeyValuePair<string, string>("client_secret", _esecConfig["EsecClientSecret"])
                };
                HttpContent content = new FormUrlEncodedContent(formData);

                var responseTask = await client.PostAsync("token", content);

                var response = responseTask.Content.ReadAsStringAsync().Result;

                _log.Information("ESEC Response: " + response);

                ValidateResponse(response);

                var userTokens = JsonConvert.DeserializeObject<UserTokensDTO>(response);

                return Ok(userTokens);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo?.Name, $"{AuthenticationError}::GetUserTokens - The authorization system is not available at the moment: {e.Message}", e);
                return Ok();
            }
        }

        /// <summary>
        ///     Sends a refresh token to ESEC, returning a new Access Token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        public async Task<IActionResult> GetRefreshToken(string refreshToken)
        {
            try
            {
                // These two lines should be removed as soon as the ESEC site's certificates start working
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler) { BaseAddress = new Uri(_esecConfig["EsecBaseAddress"]) };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", _esecConfig["EsecClientId"]),
                    new KeyValuePair<string, string>("client_secret", _esecConfig["EsecClientSecret"])
                };
                HttpContent content = new FormUrlEncodedContent(formData);

                var query = $"?grant_type=refresh_token&refresh_token={WebUtility.UrlDecode(refreshToken)}";

                var responseTask = await client.PostAsync("token" + query, content);
                var response = responseTask.Content.ReadAsStringAsync().Result;

                ValidateResponse(response);

                return Ok(response);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo?.Name, $"{AuthenticationError}::GetRefreshToken - {e.Message}", e);
                return Ok();
            }

        }

        /// <summary>
        ///     Sends an access or refresh token to the revocation endpoint, preventing the token
        ///     from ever being used again.
        /// </summary>
        /// <param name="token">Access or Refresh Token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RevokeToken/Access/{token}")]
        [Route("RevokeToken/Refresh/{token}")]
        public IActionResult RevokeToken(string token)
        {
            try
            {
                // These two lines should be removed as soon as the ESEC site's certificates start working
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler) { BaseAddress = new Uri(_esecConfig["EsecBaseAddress"]) };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", _esecConfig["EsecClientId"]),
                    new KeyValuePair<string, string>("client_secret", _esecConfig["EsecClientSecret"])
                };
                HttpContent content = new FormUrlEncodedContent(formData);

                var query = $"?token={WebUtility.UrlDecode(token)}";

                var responseTask = client.PostAsync("revoke" + query, content);
                responseTask.Wait();
                if (responseTask.Result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok();
                }

                var response = responseTask.Result.Content.ReadAsStringAsync().Result;
                ValidateResponse(response);

                return Ok(response);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo?.Name, $"{AuthenticationError}::RevokeToken - {e.Message}", e);
                return Ok();
            }
        }

        /// <summary>
        ///     Prevents an id token from being accepted by the application again. ID tokens are not
        ///     validated by the ESEC server, so they cannot be invalidated in the same way as
        ///     refresh or access tokens. Instead, we must locally keep track of them.
        /// </summary>
        [HttpPost]
        [Route("RevokeToken/Id")]
        public IActionResult RevokeIdToken()
        {
            try
            {
                // A JWT is too large to store in the URL, so it is passed in the authorization header.
                var tokenParts = Request.Headers["Authorization"].ToString().Split(" ");
                var idToken = tokenParts.Count() > 1 ? tokenParts[1] : string.Empty;
                if (!string.IsNullOrEmpty(idToken))
                {
                    EsecSecurity.RevokeToken(idToken);
                }
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo?.Name, $"{AuthenticationError}::RevokeToken - {e.Message}", e);
                return Ok();
            }
        }

        [HttpGet]
        [Route("GetHasAdminAccess")]
        [Authorize(Policy = Policy.AdminUser)]
        public async Task<IActionResult> GetHasAdminAccess()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("GetActiveStatus")]
        public async Task<IActionResult> GetActiveStatus()
        {
            var idToken = ContextAccessor?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var userToken = handler.ReadJwtToken(idToken);
            var userName = "";
            var userNameClaim = userToken.Claims.FirstOrDefault(claim => claim.Type == "name");
            if (userNameClaim == null)
            {
                var subClaim = userToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
                if (subClaim == null)
                {
                    return Unauthorized("The token does not contain a 'username' claim.");
                }

                userName = SecurityFunctions.ParseLdap(userToken.GetClaimValue("sub"))[0];
            }
            else
            {
                userName = userNameClaim.Value;
            }
            

            // Get user
            var user = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userName);

            if (user == null)
            {
                // If user is not found
                return NotFound("User not found");
            }

            if (user.ActiveStatus == true)
            {
                // If user is active
                return Ok(true);
            }
            else if (user.ActiveStatus != true && user.ActiveStatus != false)
            {
                // If user is not active or inacative/First time logging in
                user.ActiveStatus = true;
                return Ok(true);
            }
            else
            {
                // If user is Inactive
                return Ok(false);
            }
        }

        /// <summary>
        ///     Checks to ensure that a response from the ESEC OIDC endpoint is not an error.
        /// </summary>
        /// <param name="response">The JSON-formatted response string</param>
        private void ValidateResponse(string response)
        {
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            if (!responseJson.ContainsKey("error"))
            {
                return;
            }
            throw new AuthenticationException(responseJson["error_description"]);
        }
    }
}
