using System;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public static class AggregationRuleFactory
    {
        public static NumericAggregationRule CreateNumericRule(Attribute attribute)
        {
            return attribute.AggregationRuleType switch
            {
                "AVERAGE" => new AverageAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }

        public static TextAggregationRule CreateTextRule(Attribute attribute)
        {
            return attribute.AggregationRuleType switch
            {
                "PREDOMINANT" => new PredominantAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
