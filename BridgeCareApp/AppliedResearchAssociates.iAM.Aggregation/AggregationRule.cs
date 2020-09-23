using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract IEnumerable<(Attribute, (int Year, T Value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
