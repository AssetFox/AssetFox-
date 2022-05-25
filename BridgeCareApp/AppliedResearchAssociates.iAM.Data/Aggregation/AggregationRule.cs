using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract IEnumerable<(Attribute attribute, (int year, T value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
