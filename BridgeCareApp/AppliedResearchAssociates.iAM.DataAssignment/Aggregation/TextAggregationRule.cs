using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public abstract class TextAggregationRule : AggregationRule<string>
    {
        public abstract override IEnumerable<(Attribute attribute, (int year, string value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
