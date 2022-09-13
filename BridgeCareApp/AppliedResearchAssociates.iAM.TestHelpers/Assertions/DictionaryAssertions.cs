using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class DictionaryAssertions
    {
        public static void Same<T, U>(
            Dictionary<T, U> expected,
            Dictionary<T, U> actual)
            where T: IEquatable<T>
            where U: IEquatable<U>
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach (var key in expected.Keys)
            {
                Assert.True(actual.ContainsKey(key), $"Dictionary should have the key {key}. It doesn't.");
                Assert.Equal(actual[key], expected[key]);
            }
        }
    }
}
