﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public class PredominantTextAggregationRule : TextAggregationRule
    {
        public override IEnumerable<(Attribute, (int, string))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute) // (int = year, string = value of the attribute)
        {
            var test = attributeData.Cast<AttributeDatum<string>>();
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                var currentYearAttributeData = test.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (attribute, (distinctYear, currentYearAttributeData
                    .GroupBy(_ => _.Value)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .Where(_ => _ != null)
                    .OrderBy(_ => _)
                    .First()));
            }
        }
    }
}
