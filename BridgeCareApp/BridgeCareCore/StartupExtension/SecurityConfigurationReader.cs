using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BridgeCareCore.StartupExtension
{
    public static class SecurityConfigurationReader
    {
        public static string GetSecurityType(IConfiguration configuration)
        {
            string securityType = configuration.GetSection("SecurityType").Value;
            return securityType;
        }
    }
}
