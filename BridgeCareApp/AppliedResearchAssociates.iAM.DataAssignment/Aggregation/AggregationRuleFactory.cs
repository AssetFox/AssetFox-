using System;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public static class AggregationRuleFactory
    {
        public static NumericAggregationRule CreateNumericRule(DataMinerAttribute attribute)
        {
            return attribute.AggregationRuleType switch
            {
                "AVERAGE" => new AverageAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }

        public static TextAggregationRule CreateTextRule(DataMinerAttribute attribute)
        {
            return attribute.AggregationRuleType switch
            {
                "PREDOMINANT" => new PredominantAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
