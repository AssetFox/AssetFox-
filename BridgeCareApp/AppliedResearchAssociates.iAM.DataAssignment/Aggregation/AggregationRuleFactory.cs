using System;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public static class AggregationRuleFactory
    {
        public static NumericAggregationRule CreateNumericRule(DataMinerAttribute attribute)
        {
            switch (attribute.AggregationRuleType)
            {
            case "AVERAGE":
                return new AverageAggregationRule();

            default:
                throw new InvalidOperationException();
            }
        }

        public static TextAggregationRule CreateTextRule(DataMinerAttribute attribute)
        {
            switch (attribute.AggregationRuleType)
            {
            case "PREDOMINANT":
                return new PredominantAggregationRule();

            default:
                throw new InvalidOperationException();
            }
        }
    }
}
