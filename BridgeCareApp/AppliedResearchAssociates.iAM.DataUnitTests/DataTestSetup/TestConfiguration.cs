using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataUnitTests
{
    public static class TestConfiguration
    {
        public static IConfiguration Get()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .AddJsonStream(GetStream())
                .Build();
            return config;
        }

        private static Stream GetStream()
        {
            const string key = "7x!z%C*F-JaNdRgUk242s5v8y,B?D(G.";
            const string keyConfig = "{\"EncryptionKey\":\"" + key + "\"}";
            return new MemoryStream(Encoding.UTF8.GetBytes(keyConfig ?? ""));
        }
    }
}
