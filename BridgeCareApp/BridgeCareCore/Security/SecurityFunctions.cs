using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BridgeCareCore.Security
{
    public static class SecurityFunctions
    {
        /// <summary>
        /// Retrieves the value of the claim of the given type from the JWT payload claims.
        /// </summary>
        /// <returns></returns>
        public static string GetClaimValue(this JwtSecurityToken jwt, string type) =>
            jwt.Claims.FirstOrDefault(claim => claim.Type == type)?.Value;

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

            using var client = new HttpClient(handler) { BaseAddress = new Uri(esecConfig["EsecBaseAddress"]) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseTask = client.GetAsync("jwks?AuthorizationProvider=OIDC Auth Provider");
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
