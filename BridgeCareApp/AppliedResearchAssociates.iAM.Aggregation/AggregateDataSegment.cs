using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AggregateDataSegment
    {
        private List<IAttributeDatum> AttributeData { get; } = new List<IAttributeDatum>();

        public AggregateDataSegment(Segment segment) => Segment = segment;

        public Segment Segment { get; }

        public void AddDatum(IAttributeDatum datum) => AttributeData.Add(datum);

        public IEnumerable<(int year, T value)> GetAggregatedValuesByYear<T>(Attribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AttributeData.Where(_ => _.Attribute.Guid == attribute.Guid);
            return aggregationRule.Apply(specifiedData);
        }
    }
}
