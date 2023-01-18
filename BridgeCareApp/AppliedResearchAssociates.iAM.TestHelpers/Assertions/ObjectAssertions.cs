using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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

        public static void Singleton<T>(T expected, object actual)
        {
            var castActual = (List<T>)actual;
            var actualSingle = castActual.Single();
            Assert.Equal(expected, actualSingle);
        }

        public static void EquivalentSingleton<T>(T expected, object actual)
        {
            var castActual = (List<T>) actual;
            var singleton = castActual.Single();
            Equivalent(expected, singleton);
        }
    }
}
