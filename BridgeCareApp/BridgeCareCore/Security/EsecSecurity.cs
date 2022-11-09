using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BridgeCareCore.Security
{
    public class EsecSecurity : IEsecSecurity
    {
        private readonly RsaSecurityKey _esecPublicKey;
        private readonly string _securityType;
        private readonly IConfiguration _config;
        private IRoleClaimsMapper _roleClaimsMapper;

        /// <summary>
        ///     Each key is a token that has been revoked. Its value is the unix timestamp of the
        ///     time at which it expires.
        /// </summary>
        private ConcurrentDictionary<string, long> _revokedTokens;

        public EsecSecurity(IConfiguration config, IRoleClaimsMapper roleClaimsMapper)
        {
            _revokedTokens = new ConcurrentDictionary<string, long>();
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _securityType = _config.GetSection("SecurityType").Value;
            _esecPublicKey = SecurityFunctions.GetPublicKey(_config.GetSection("EsecConfig"));
            _roleClaimsMapper = roleClaimsMapper ?? throw new ArgumentNullException(nameof(roleClaimsMapper));
        }

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
            var idToken = request.Headers["Authorization"].ToString().Split(" ")[1];

            if (string.IsNullOrEmpty(idToken))
            {
                throw new UnauthorizedAccessException("No authorization bearer present on request.");
            }

            if (_revokedTokens.ContainsKey(idToken))
            {
                throw new UnauthorizedAccessException("Your ID Token has been revoked.");
            }

            var decodedToken = DecodeToken(idToken);
            var claimsPrincipal = request.HttpContext.User;

            if (_securityType == SecurityConstants.SecurityTypes.Esec)
            {
                var roleStrings = SecurityFunctions.ParseLdap(decodedToken.GetClaimValue("roles"));
                if (roleStrings.Count == 0)
                {
                    throw new UnauthorizedAccessException("User has no security roles assigned.");
                }

                var internalRoles = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.Esec, roleStrings);
                var HasAdminAccess = _roleClaimsMapper.HasAdminAccess(claimsPrincipal);
                var HasSimulationAccess = _roleClaimsMapper.HasSimulationAccess(claimsPrincipal);

                return new UserInfo
                {
                    Name = SecurityFunctions.ParseLdap(decodedToken.GetClaimValue("sub"))[0],                    
                    Email = decodedToken.GetClaimValue("email"),
                    HasAdminAccess = HasAdminAccess,
                    HasSimulationAccess = HasSimulationAccess,
                    InternalRoles = internalRoles,
                };
            }

            if (_securityType == SecurityConstants.SecurityTypes.B2C)
            {
                var internalRoles = _roleClaimsMapper.GetInternalRoles(SecurityConstants.SecurityTypes.B2C, new List<string>
                { SecurityConstants.Role.Administrator });
                var HasAdminAccess = _roleClaimsMapper.HasAdminAccess(claimsPrincipal);
                var HasSimulationAccess = _roleClaimsMapper.HasSimulationAccess(claimsPrincipal);

                return new UserInfo
                {
                    Name = decodedToken.GetClaimValue("name"),
                    Email = decodedToken.GetClaimValue("email"),
                    HasAdminAccess = HasAdminAccess, 
                    HasSimulationAccess = HasSimulationAccess, 
                    InternalRoles = internalRoles,
                };
            }

            return new UserInfo { Name = "", Email = "" };
        }

        /// <summary>
        ///     Given a dictionary version of the LDAP-formatted JSON from ESEC, produces a UserInfo
        ///     object containing only the user's name, email, and relevant role
        /// </summary>
        /// <param name="userInformationDictionary"></param>
        /// <returns></returns>
        public UserInfo GetUserInformation(Dictionary<string, string> userInformationDictionary)
        {
            var name = SecurityFunctions.ParseLdap(userInformationDictionary["sub"])[0];
            var email = userInformationDictionary.ContainsKey("email") ? userInformationDictionary["email"] : null;
            return new UserInfo { Name = name, Email = email };
        }

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
                ValidateLifetime = true
            };

            var handler = new JwtSecurityTokenHandler();

            if (_securityType == SecurityConstants.SecurityTypes.Esec)
            {
                validationParameters.IssuerSigningKey = _esecPublicKey;

                handler.ValidateToken(idToken, validationParameters, out var validatedToken);
                return validatedToken as JwtSecurityToken;
            }

            var token = handler.ReadJwtToken(idToken);
            return token;
        }
    }
}
