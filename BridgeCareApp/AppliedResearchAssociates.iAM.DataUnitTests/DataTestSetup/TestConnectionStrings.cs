using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataUnitTests
{
    public static class TestConnectionStrings
    {
        public static string BridgeCare(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("BridgeCareConnex");
            return connectionString;
        }
    }
}
