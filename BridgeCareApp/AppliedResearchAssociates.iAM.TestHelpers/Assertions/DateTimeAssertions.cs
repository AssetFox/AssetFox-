using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.TestHelpers.Assertions
{
    public static class DateTimeAssertions
    {
        public static void Between(DateTime expectedMinimum, DateTime expectedMaximum, DateTime actual, TimeSpan errorTolerance = new())
        {
            var minimumWithErrorTolerance = expectedMinimum - errorTolerance;
            var maximumWithErrorTolerance = expectedMaximum + errorTolerance;
            Assert.True(actual >= minimumWithErrorTolerance, $"Time should be on or after {minimumWithErrorTolerance}, but it is {actual}");
            Assert.True(actual <= maximumWithErrorTolerance, $"Time should be on or before {maximumWithErrorTolerance}, but it is {actual}");
        }
    }
}
