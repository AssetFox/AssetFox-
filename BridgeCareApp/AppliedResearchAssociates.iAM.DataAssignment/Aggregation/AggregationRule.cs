using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract IEnumerable<(Attribute attribute, (int year, T value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
