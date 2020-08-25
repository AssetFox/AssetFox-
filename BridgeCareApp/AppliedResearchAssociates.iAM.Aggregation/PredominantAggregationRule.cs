using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class PredominantAggregationRule : AggregationRule<string>
    {
        public override IEnumerable<(int, string)> Apply(IEnumerable<IAttributeDatum> attributeData)
        {
            var test = attributeData.Cast<AttributeDatum<string>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeData = test.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (distinctYear, currentYearAttributeData
                    .GroupBy(_ => _.Value)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .Where(_ => _ != null)
                    .OrderBy(_ => _)
                    .First());
            }
        }
    }
}
