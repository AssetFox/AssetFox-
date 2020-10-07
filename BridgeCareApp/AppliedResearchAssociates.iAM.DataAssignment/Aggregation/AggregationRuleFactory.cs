using System;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public static class AggregationRuleFactory
    {
        public static NumericAggregationRule CreateNumericRule(Attribute attribute)
        {
            switch (attribute.DataType)
            {
            case "AVERAGE":
                return new AverageAggregationRule();

            default:
                throw new InvalidOperationException();
            }
        }

        public static TextAggregationRule CreateTextRule(Attribute attribute)
        {
            switch (attribute.DataType)
            {
            case "PREDOMINANT":
                return new PredominantAggregationRule();

            default:
                throw new InvalidOperationException();
            }
        }
    }
}
