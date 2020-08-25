using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AverageAggregationRule : AggregationRule<double>
    {
        public override IEnumerable<(int, double)> Apply(IEnumerable<IAttributeDatum> attributeData)
        {
            var test = attributeData.Cast<AttributeDatum<double>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeData = test.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (distinctYear, currentYearAttributeData.Select(_ => _.Value).Average());
            }
        }
    }
}
