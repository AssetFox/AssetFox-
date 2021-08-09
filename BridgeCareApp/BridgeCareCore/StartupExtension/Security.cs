

using AppliedResearchAssociates.iAM.DataPersistenceCore;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BridgeCareCore.StartupExtension
{
    public static class Security
    {
        public static void AddSecurityConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            var securityType = Configuration.GetSection("SecurityType").Value;

            if (securityType == SecurityConstants.SecurityTypes.Esec)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            RequireExpirationTime = true,
                            RequireSignedTokens = true,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            IssuerSigningKey = SecurityFunctions.GetPublicKey(Configuration.GetSection("EsecConfig"))
                        };
                    });
            }

            if (securityType == SecurityConstants.SecurityTypes.B2C)
            {
                services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                    .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy(SecurityConstants.Policy.AdminOrDistrictEngineer,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator, Role.DistrictEngineer)));
                options.AddPolicy(SecurityConstants.Policy.Admin,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator)));
            });

            services.AddSingleton<IEsecSecurity, EsecSecurity>();
            services.AddSingleton<IAuthorizationHandler, RestrictAccessHandler>();
        }
    }
}
