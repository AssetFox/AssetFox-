﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BridgeCareCoreBaseController
    {
        private static IConfigurationSection _esecConfig;
        private readonly ILog _log;

        public AuthenticationController(IConfiguration config, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, ILog log) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
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
                ValidateResponse(response);
                var userInfo = JsonConvert.DeserializeObject<UserInfoDTO>(response);
                userInfo.HasAdminClaim = UserInfo.HasAdminClaim;
                return Ok(userInfo);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Fetches user info as a JSON-formatted string from ESEC
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>JSON-formatted user info</returns>
        private static string GetUserInfoString(string token)
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

            var responseTask = client.PostAsync("userinfo", content);
            responseTask.Wait();

            return responseTask.Result.Content.ReadAsStringAsync().Result;
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
                 //responseTask.Wait();

                var response = responseTask.Content.ReadAsStringAsync().Result;

                ValidateResponse(response);

                var userTokens = JsonConvert.DeserializeObject<UserTokensDTO>(response);

                return Ok(userTokens);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Sends a refresh token to ESEC, returning a new Access Token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        public IActionResult GetRefreshToken(string refreshToken)
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

                var responseTask = client.PostAsync("token" + query, content);
                responseTask.Wait();

                var response = responseTask.Result.Content.ReadAsStringAsync().Result;

                ValidateResponse(response);

                return Ok(response);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return StatusCode(500, e.Message);
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
                _log.Error(e.Message);
                return StatusCode(500, e.Message);
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
                var idToken = Request.Headers["Authorization"].ToString().Split(" ")[1];
                EsecSecurity.RevokeToken(idToken);
                return Ok();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return StatusCode(500, e.Message);
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
