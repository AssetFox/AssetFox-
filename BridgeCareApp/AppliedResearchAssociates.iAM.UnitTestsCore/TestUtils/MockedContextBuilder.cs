using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class MockedContextBuilder
    {
        /// <summary>
        /// Adds a mocked dataset to an existing mocked context
        /// </summary>
        /// <param name="context">The existing context that the source will be added to</param>
        /// <param name="contextProperty">The property on the context that returns the source</param>
        /// <param name="source">A queryable source of data</param>
        /// <returns>A reference to the mocked DBSet for modification or use in Verify assertions</returns>
        public static Mock<DbSet<T>> AddDataSet<C, T>(Mock<C> context, System.Linq.Expressions.Expression<Func<C, DbSet<T>>> contextProperty, IQueryable<T> source)
            where T : class
            where C : DbContext
        {
            // From https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
            var newDataSet = new Mock<DbSet<T>>();
            newDataSet.As<IQueryable<T>>().Setup(_ => _.Provider).Returns(source.Provider);
            newDataSet.As<IQueryable<T>>().Setup(_ => _.Expression).Returns(source.Expression);
            newDataSet.As<IQueryable<T>>().Setup(_ => _.ElementType).Returns(source.ElementType);
            newDataSet.As<IQueryable<T>>().Setup(_ => _.GetEnumerator()).Returns(source.GetEnumerator());

            context.Setup(contextProperty).Returns(newDataSet.Object);
            context.Setup(_ => _.Set<T>()).Returns(newDataSet.Object);

            return newDataSet;
        }

        public static void AddConfigurationKeys(Mock<IConfiguration> configuration, string keyName, List<string> configurationValues)
        {
            var mockedKeySection = new Mock<IConfigurationSection>();
            var mockedKeySectionList = new List<IConfigurationSection>();
            foreach (var value in configurationValues)
            {
                var section = new Mock<IConfigurationSection>();
                section.Setup(_ => _.Value).Returns(value);
                mockedKeySectionList.Add(section.Object);
            }
            mockedKeySection.Setup(_ => _.GetChildren()).Returns(mockedKeySectionList.AsEnumerable());
            configuration.Setup(_ => _.GetSection(keyName)).Returns(mockedKeySection.Object);
        }
    }
}
