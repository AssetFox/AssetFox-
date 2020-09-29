using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public static class Aggregator
    {
        public static List<AssignedDataSegment> AssignAttributeDataToSegments(
            IEnumerable<IAttributeDatum> attributeData,
            IEnumerable<Segment> networkSegments)
        {
            var assignedDataSegments = new List<AssignedDataSegment>();

            // Copy the network segments into a new list of AggregateDataSegments
            foreach (var networkSegment in networkSegments)
            {
                assignedDataSegments.Add(new AssignedDataSegment(networkSegment));
            }

            foreach (var datum in attributeData)
            {
                AssignedDataSegment matchingLocationSegment =
                    assignedDataSegments.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Segment.Location));

                if (matchingLocationSegment != null)
                {
                    matchingLocationSegment.AddDatum(datum);
                }
                else
                {
                    // TODO: No matching segment for the current data. What do we do?
                }
            }
            return assignedDataSegments;
        }
    }
}
