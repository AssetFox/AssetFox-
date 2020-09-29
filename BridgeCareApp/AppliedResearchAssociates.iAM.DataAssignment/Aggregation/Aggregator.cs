using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public static class Aggregator
    {
        public static List<Segment> AssignAttributeDataToSegments(
            IEnumerable<IAttributeDatum> attributeData,
            IEnumerable<Segment> networkSegments)
        {
            foreach (var datum in attributeData)
            {
                Segment matchingLocationSegment =
                    networkSegments.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Location));

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
