using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests
{
    public static class TestConfiguration
    {
        public static IConfiguration Get()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();
            return config;
        }
    }
}
