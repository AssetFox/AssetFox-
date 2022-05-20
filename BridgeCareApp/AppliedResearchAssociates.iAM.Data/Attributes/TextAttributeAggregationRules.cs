using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    internal class TextAttributeAggregationRules
    {
        public const string Predominant = "PREDOMINANT";
        public static IEnumerable<string> ValidRuleNames()
        {
            yield return Predominant;
        }
    }
}
