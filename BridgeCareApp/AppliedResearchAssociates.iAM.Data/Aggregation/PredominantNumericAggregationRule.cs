using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

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
                var maxCount = currentYearAttributeData
                    .GroupBy(_ => _.Value)
                    .Select(_ => new { Key = _.Key, RecordCount = _.Count() })
                    .Max(_ => _.RecordCount);
                yield return (attribute, (distinctYear, currentYearAttributeData
                    .GroupBy(_ => _.Value)
                    .Select(_ => new { Key = _.Key, RecordCount = _.Count() })
                    .Where(_ => _.RecordCount == maxCount)
                    .Select(_ => _.Key)
                    .OrderByDescending(_ => _)
                    .First()));
            }
        }
    }
}
