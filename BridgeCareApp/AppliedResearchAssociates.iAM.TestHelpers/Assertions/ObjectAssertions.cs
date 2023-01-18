using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Equivalency;  // Licensed under Apache 2.0. Seems to be compatible with AGPL 3.
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class ObjectAssertions
    {
        public static void Equivalent(object expected, object actual)
        {
            actual.Should().BeEquivalentTo(expected);
        }

        public static void EquivalentExcluding<T>(T expected, T actual, params Expression<Func<T, object>>[] exclusions)
             where T : class 
            => actual.Should().BeEquivalentTo(expected, t => exclusionHelper(t, exclusions));

        private static EquivalencyAssertionOptions<T> exclusionHelper<T>(EquivalencyAssertionOptions<T> assertionOptions, params Expression<Func<T, object>>[] exclusions)
        {
            foreach (var excl in exclusions)
            {
                assertionOptions.Excluding(excl);
            }

            return assertionOptions;
        }

        public static void Singleton<T>(T expectedSingleEntry, object actualEnumerable)
        {
            var castActual = (IEnumerable<T>)actualEnumerable;
            var actualSingle = castActual.Single();
            Assert.Equal(expectedSingleEntry, actualSingle);
        }

        public static void EquivalentSingleton<T>(T expectedSingleEntry, object actualEnumerable)
        {
            var castActual = (List<T>)actualEnumerable;
            var singleton = castActual.Single();
            Equivalent(expectedSingleEntry, singleton);
        }
    }
}
