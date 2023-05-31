using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public class AddAggregationRule : NumericAggregationRule
    {
        public override IEnumerable<(Attribute attribute, (int year, double value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute)
        {
            var data = attributeData.Cast<AttributeDatum<double>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeDate = data.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (attribute, (distinctYear, currentYearAttributeDate
                    .Sum(_ => _.Value)));
            }
        }
    }
}
