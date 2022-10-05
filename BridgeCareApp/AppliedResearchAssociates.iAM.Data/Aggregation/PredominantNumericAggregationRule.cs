using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public class PredominantNumericAggregationRule : NumericAggregationRule
    {
        public override IEnumerable<(Attribute attribute, (int year, double value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute)
        {
            var test = attributeData.Cast<AttributeDatum<double>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeData = test.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (attribute, (distinctYear, currentYearAttributeData
                    .GroupBy(_ => _.Value)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .OrderByDescending(_ => _)
                    .First()));
            }
        }
    }
}
