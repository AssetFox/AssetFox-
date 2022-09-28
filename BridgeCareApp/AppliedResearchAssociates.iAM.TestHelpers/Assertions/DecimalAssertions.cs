using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class DecimalAssertions
    {
        public static void ApproximatelyEqual(decimal expected, decimal actual, decimal allowedError = 0.01m)
        {
            var difference = Math.Abs(expected - actual);
            Assert.True(difference <= allowedError);
        }
    }
}
