using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BridgeCareCoreTests.Helpers
{
    public static class ActionResultAssertions
    {
        public static object OkObject(IActionResult result)
        {
            Assert.IsType<OkObjectResult>(result);
            var castResult = (OkObjectResult)result;
            return castResult.Value;
        }

        public static void Ok(IActionResult result)
        {
            Assert.IsType<OkResult>(result);
        }

        public static void BadRequest(IActionResult result)
        {
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
