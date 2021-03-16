﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILog _log;
        private static IConfigurationSection _esecConfig;
        private static IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public AuthenticationController(ILog log, IConfiguration config, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _esecConfig = config?.GetSection("EsecConfig") ?? throw new ArgumentNullException(nameof(config));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        /// <summary>
        /// API endpoint for fetching user info from ESEC using the OpenID Connect protocol
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
                AddUser(userInfo);
                return Ok(userInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        private void AddUser(UserInfoDTO userInfo)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                _unitOfDataPersistenceWork.UserRepo.AddUser(SecurityFunctions.ParseLdap(userInfo.Sub)[0],
                    SecurityFunctions.ParseLdap(userInfo.Roles)[0]);
                _unitOfDataPersistenceWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Fetches user info as a JSON-formatted string from ESEC
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

            using var client = new HttpClient(handler) {BaseAddress = new Uri(_esecConfig["ESECBaseAddress"])};
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
        /// Fetches user info as a dictionary
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>User info dictionary</returns>
        public static Dictionary<string, string> GetUserInfoDictionary(string token) =>
            JsonConvert.DeserializeObject<Dictionary<string, string>>(GetUserInfoString(token));

        public static UserInfo GetUserInformation(Dictionary<string, string> userInfoDictionary) =>
            _esecSecurity.GetUserInformation(userInfoDictionary);

        /// <summary>
        /// API endpoint for fetching ID and Access tokens from ESEC using the OpenID Connect protocol
        /// </summary>
        /// <param name="code">The authentication or error code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserTokens/{code}")]
        public IActionResult GetUserTokens(string code)
        {
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler) {BaseAddress = new Uri(_esecConfig["EsecBaseAddress"])};
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

            var responseTask = client.PostAsync("token", content);
            responseTask.Wait();

            var response = responseTask.Result.Content.ReadAsStringAsync().Result;

            ValidateResponse(response);

            var userTokens = JsonConvert.DeserializeObject<UserTokensDTO>(response);

            return Ok(userTokens);
        }

        /// <summary>
        /// Sends a refresh token to ESEC, returning a new Access Token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        public IActionResult GetRefreshToken(string refreshToken)
        {
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler) {BaseAddress = new Uri(_esecConfig["EsecBaseAddress"])};
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

        /// <summary>
        /// Sends an access or refresh token to the revocation endpoint, preventing the token from ever being used again.
        /// </summary>
        /// <param name="token">Access or Refresh Token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RevokeToken/Access/{token}")]
        [Route("RevokeToken/Refresh/{token}")]
        public IActionResult RevokeToken(string token)
        {
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler) {BaseAddress = new Uri(_esecConfig["EsecBaseAddress"])};
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

        /// <summary>
        /// Prevents an id token from being accepted by the application again.
        /// ID tokens are not validated by the ESEC server, so they cannot be invalidated in the
        /// same way as refresh or access tokens. Instead, we must locally keep track of them.
        /// </summary>
        [HttpPost]
        [Route("RevokeToken/Id")]
        public IActionResult RevokeIdToken()
        {
            // A JWT is too large to store in the URL, so it is passed in the authorization header.
            string idToken = Request.Headers["Authorization"];
            _esecSecurity.RevokeToken(idToken);
            return Ok();
        }

        /// <summary>
        /// Checks to ensure that a response from the ESEC OIDC endpoint is not an error.
        /// </summary>
        /// <param name="response">The JSON-formatted response string</param>
        private void ValidateResponse(string response)
        {
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            if (!responseJson.ContainsKey("error"))
            {
                return;
            }

            _log.Error($"ESEC endpoint returned error - {responseJson["error"]}: {responseJson["error_description"]}");
            throw new AuthenticationException(responseJson["error_description"]);
        }
    }
}
