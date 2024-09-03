using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BridgeCareCore.Security
{
    public static class SecurityFunctions
    {
        public static UserInfoDTO ToDto(this UserInfo userInfo) =>
            new UserInfoDTO { Sub = userInfo.Name, HasAdminAccess  = userInfo.HasAdminAccess, HasSimulationAccess = userInfo.HasSimulationAccess, Email = userInfo.Email };

        /// <summary>
        ///     Retrieves the value of the claim of the given type from the JWT payload claims.
        /// </summary>
        /// <returns></returns>
        public static string GetClaimValue(this JwtSecurityToken jwt, string type) =>
            jwt.Claims.FirstOrDefault(claim => claim.Type == type)?.Value;

        /// <summary>
        ///     Given an LDAP-formatted string from ESEC, extracts the Common Name (CN) fields.
        /// </summary>
        /// <param name="ldap"></param>
        /// <returns>Role</returns>
        public static List<string> ParseLdap(string ldap)
        {
            if (string.IsNullOrEmpty(ldap))
            {
                return new List<string>();
            }

            var segments = ldap.Split('^');
            var commonNameSegments = segments.Select(segment => segment.Split(',')[0]);
            var commonNames = commonNameSegments.Select(segment => segment.Split('=')[1]);
            return commonNames.ToList();
        }

        /// <summary>
        ///     Fetches the public key information from the ESEC jwks endpoint, and generates an
        ///     RsaSecurityKey from it
        /// </summary>
        public static RsaSecurityKey GetPublicKey(IConfiguration esecConfig)
        {
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var baseAddress = esecConfig["EsecBaseAddress"];
            using var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseTask = client.GetAsync("jwks?AuthorizationProvider=" + esecConfig["EsecOIDCProvider"]);
            responseTask.Wait();

            var resultJson = responseTask.Result.Content.ReadAsStringAsync().Result;

            // This dictionary and list structure matches the structure of the JSON response from
            // the ESEC JWKS endpoint
            var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(resultJson);

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters()
            {
                Modulus = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["n"]),
                Exponent = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["e"])
            });
            return new RsaSecurityKey(rsa);
        }
    }
}
