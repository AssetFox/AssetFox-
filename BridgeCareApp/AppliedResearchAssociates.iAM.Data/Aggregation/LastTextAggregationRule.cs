using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public class LastTextAggregationRule : TextAggregationRule
    {
        public override IEnumerable<(Attribute attribute, (int year, string value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attributes.Attribute attribute)
        {
            var test = attributeData.Cast<AttributeDatum<string>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeDate = test.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (attribute, (distinctYear, currentYearAttributeDate
                    .OrderBy(__ => __.TimeStamp)
                    .Select(__ => __.Value)
                    .First()));
            }
        }
    }
}
