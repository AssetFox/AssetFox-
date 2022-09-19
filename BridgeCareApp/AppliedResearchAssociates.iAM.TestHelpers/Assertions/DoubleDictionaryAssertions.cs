using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class DoubleDictionaryAssertions
    {
        public static void ApproximatelySame<T>(
            Dictionary<T, double> expected,
            Dictionary<T, double> actual,
            double tolerance = 1E-10)
            where T: IEquatable<T>
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach (var key in expected.Keys)
            {
                Assert.True(actual.ContainsKey(key), $"Dictionary should have the key {key}. It doesn't.");
                DoubleAssertions.ApproximatelyEqual(expected[key], actual[key], tolerance);
            }
        }
    }
}
