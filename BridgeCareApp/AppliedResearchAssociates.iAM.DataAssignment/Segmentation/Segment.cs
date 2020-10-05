using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataAssignment.Segmentation
{
    public class Segment
    {
        public Segment(Location segmentLocation)
        {
            Location = segmentLocation;
        }
        public IEnumerable<(Attribute attribute, (int year, T value))> GetAggregatedValuesByYear<T>(Attribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AssignedData.Where(_ => _.Attribute.Id == attribute.Id);
            return aggregationRule.Apply(specifiedData, attribute);
        }
        public void AssignAttributeData(IEnumerable<IAttributeDatum> attributeData)
        {
            foreach (var datum in attributeData)
            {
                if (datum.Location.MatchOn(Location))
                {
                    AssignedData.Add(datum);
                }
                else
                {
                    // TODO: No matching segment for the current data. What do we do?
                }
            }
        }
        public List<IAttributeDatum> AssignedData { get; } = new List<IAttributeDatum>();
        public Location Location { get; }
    }
}
