﻿using System;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public static class AggregationRuleFactory
    {
        public static NumericAggregationRule CreateNumericRule(Attribute attribute)
        {
            return attribute.AggregationRuleType.ToUpper() switch
            {
                "AVERAGE" => new AverageAggregationRule(),
                "LAST" => new LastNumericAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }

        public static TextAggregationRule CreateTextRule(Attribute attribute)
        {
            return attribute.AggregationRuleType.ToUpper() switch
            {
                "PREDOMINANT" => new PredominantAggregationRule(),
                "LAST" => new LastTextAggregationRule(),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
