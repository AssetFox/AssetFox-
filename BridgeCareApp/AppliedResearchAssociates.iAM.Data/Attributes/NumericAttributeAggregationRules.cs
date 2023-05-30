using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public static class NumericAttributeAggregationRules
    {
        public const string Average = "AVERAGE";
        public const string Last = "LAST";
        public const string Add = "ADD";
        static string[] ValidRuleList = new string[] {
            Average, Last, Add
        };


        public static IEnumerable<string> ValidRules()
        {
            return ValidRuleList;
        }
    }
}
