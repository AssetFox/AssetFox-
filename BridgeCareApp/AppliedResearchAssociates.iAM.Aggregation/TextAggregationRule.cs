using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class TextAggregationRule : AggregationRule<string>
    {
        public abstract override IEnumerable<(Attribute attribute, (int year, string value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
