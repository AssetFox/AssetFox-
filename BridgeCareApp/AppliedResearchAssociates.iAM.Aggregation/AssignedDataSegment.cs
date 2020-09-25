using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AssignedDataSegment
    {
        private List<IAttributeDatum> AssignedData { get; } = new List<IAttributeDatum>();

        public AssignedDataSegment(Segment segment) => Segment = segment;

        public Segment Segment { get; }

        public void AddDatum(IAttributeDatum datum) => AssignedData.Add(datum);

        public IEnumerable<(Attribute attribute, (int year, T value))> GetAggregatedValuesByYear<T>(Attribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AssignedData.Where(_ => _.Attribute.Guid == attribute.Guid);
            return aggregationRule.Apply(specifiedData, attribute);
        }
    }
}
