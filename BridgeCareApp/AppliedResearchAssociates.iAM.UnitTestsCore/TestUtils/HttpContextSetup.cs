using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class HttpContextSetup
    {
        public static void AddAuthorizationHeader(DefaultHttpContext context) =>
            context.Request.Headers.Add("Authorization", "Bearer abc123");

    }
}
