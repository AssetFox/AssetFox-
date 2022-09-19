using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class DoubleAssertions
    {
        public static void ApproximatelyEqual(double expected, double actual, double tolerance = 1E-10)
        {
            var difference = Math.Abs(expected - actual);
            Assert.True(difference <= tolerance);
        }
    }
}
