using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class NullableDoubleAssertions
    {
        public static void ApproximatelyEqual(double? expected, double? actual, double tolerance = 1E-10)
        {
            var expectedIsNull = expected == null;
            var actualIsNull = actual == null;
            Assert.Equal(expectedIsNull, actualIsNull);
            if (!expectedIsNull)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                DoubleAssertions.ApproximatelyEqual(expected.Value, actual.Value, tolerance);
#pragma warning restore CS8629 // Nullable value type may be null.
            }
        }
    }
}
