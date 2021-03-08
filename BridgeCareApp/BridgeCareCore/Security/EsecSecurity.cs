using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using BridgeCare.Security;
using BridgeCareCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoreLinq;
using Newtonsoft.Json;

namespace BridgeCareCore.Security
{
    public class EsecSecurity
    {
        private readonly RsaSecurityKey _esecPublicKey;

        /// <summary>
        ///     Each key is a token that has been revoked. Its value is the unix timestamp of the
        ///     time at which it expires.
        /// </summary>
        private ConcurrentDictionary<string, long> _revokedTokens;

        public EsecSecurity(IConfiguration config)
        {
            _revokedTokens = new ConcurrentDictionary<string, long>();
            var esecConfig = config?.GetSection("EsecConfig") ?? throw new ArgumentNullException(nameof(config));
            _esecPublicKey = SecurityFunctions.GetPublicKey(esecConfig);
        }

        /// <summary>
        ///     Checks if the provided token has been revoked.
        /// </summary>
        /// <param name="idToken">JWT ID Token</param>
        /// <returns>bool</returns>
        private bool TokenIsRevoked(string idToken) => _revokedTokens.ContainsKey(idToken);

        /// <summary>
        ///     Prevents the parser from accepting the provided token in the future.
        /// </summary>
        /// <param name="idToken">The JWT ID Token</param>
        public void RevokeToken(string idToken)
        {
            RemoveExpiredTokens();
            var decodedToken = DecodeToken(idToken);
            var expirationString = decodedToken.GetClaimValue("exp");
            var expiration = long.Parse(expirationString);
            _revokedTokens.TryAdd(idToken, expiration);
        }

        /// <summary>
        ///     Removes all expired tokens from the revokedTokens dictionary, as they no longer need
        ///     to be tracked. This keeps the dictionary from endlessly growing as the application runs
        /// </summary>
        private void RemoveExpiredTokens()
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _revokedTokens =
                new ConcurrentDictionary<string, long>(_revokedTokens.Where(entry => entry.Value > currentTime));
        }

        /// <summary>
        ///     Given an id_token from ESEC, validates it and extracts the User's Information
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserInfo GetUserInformation(HttpRequest request)
        {
            var idToken = request.Headers["Authorization"].Split(" ").Last().ToString();
            if (TokenIsRevoked(idToken))
            {
                throw new UnauthorizedAccessException("Your ID Token has been revoked.");
            }

            var decodedToken = DecodeToken(idToken);
            var roleStrings = ParseLdap(decodedToken.GetClaimValue("roles"));
            if (roleStrings.Count == 0)
            {
                throw new UnauthorizedAccessException("User has no security roles assigned.");
            }

            var role = roleStrings.First(roleString => Role.AllValidRoles.Contains(roleString));
            var name = ParseLdap(decodedToken.GetClaimValue("sub"))[0];
            var email = decodedToken.GetClaimValue("email");
            return new UserInfo { Name = name, Role = role, Email = email };
        }

        /// <summary>
        ///     Given a dictionary version of the LDAP-formatted JSON from ESEC, produces a UserInfo
        ///     object containing only the user's name, email, and relevant role
        /// </summary>
        /// <param name="userInformationDictionary"></param>
        /// <returns></returns>
        public UserInfo GetUserInformation(Dictionary<string, string> userInformationDictionary)
        {
            var role = ParseLdap(userInformationDictionary["roles"])
                .First(roleString => Role.AllValidRoles.Contains(roleString));
            var name = ParseLdap(userInformationDictionary["sub"])[0];
            var email = userInformationDictionary.ContainsKey("email") ? userInformationDictionary["email"] : null;
            return new UserInfo { Name = name, Role = role, Email = email };
        }

        /*public UserInfo GetUserInformation(HttpRequest request)
        {
            string GetValue(string key) => request.Headers.GetValues(key).First();
            return new UserInfo { Name = GetValue("Name"), Role = GetValue("Role"), Email = GetValue("Email") };
        }*/

        /// <summary>
        ///     Creates a JwtSecurityToken object from a JWT string.
        /// </summary>
        /// <param name="idToken">JWT string</param>
        private JwtSecurityToken DecodeToken(string idToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = _esecPublicKey
            };

            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(idToken, validationParameters, out var validatedToken);
            return validatedToken as JwtSecurityToken;
        }

        /// <summary>
        ///     Given an LDAP-formatted string from ESEC, extracts the Common Name (CN) fields.
        /// </summary>
        /// <param name="ldap"></param>
        /// <returns>Role</returns>
        public List<string> ParseLdap(string ldap)
        {
            if (string.IsNullOrEmpty(ldap))
            {
                return new List<string>();
            }

            var segments = ldap.Split('^');
            var commonNameSegments = segments.Select(segment => segment.Split(',')[0]);
            //var validCommonNameSegments = commonNameSegments.Where(segment => segment.Contains('='));
            var commonNames = commonNameSegments.Select(segment => segment.Split('=')[1]);
            return commonNames.ToList();
        }
    }
}
