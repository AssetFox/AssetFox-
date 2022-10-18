using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BridgeCareCoreTests.Helpers
{
    public static class ActionResultAssertions
    {
        public static void OkObject(IActionResult result)
        {
            Assert.IsType<OkObjectResult>(result);
        }

        public static void Ok(IActionResult result)
        {
            Assert.IsType<OkResult>(result);
        }
    }
}
