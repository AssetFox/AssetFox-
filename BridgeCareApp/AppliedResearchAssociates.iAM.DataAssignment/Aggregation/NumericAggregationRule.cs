using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public abstract class NumericAggregationRule : AggregationRule<double>
    {
        public abstract override IEnumerable<(Attribute attribute, (int year, double value))> Apply(IEnumerable<IAttributeDatum> attributeData, Attribute attribute);
    }
}
