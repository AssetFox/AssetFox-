using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public static class NumericAttributeAggregationRules
    {
        public const string Predominant = "PREDOMINANT";

        public static IEnumerable<string> ValidRules()
        {
            yield return Predominant;
        }
    }
}
