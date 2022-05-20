using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public static class NumericAttributeAggregationRules
    {
        public const string Average = "AVERAGE";

        public static IEnumerable<string> ValidRules()
        {
            yield return Average;
        }
    }
}
