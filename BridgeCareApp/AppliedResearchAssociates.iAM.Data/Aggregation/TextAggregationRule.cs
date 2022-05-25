using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public abstract class TextAggregationRule : AggregationRule<string>
    {
        public abstract override IEnumerable<(Attribute attribute, (int year, string value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
