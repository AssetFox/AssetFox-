﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public static class Aggregator
    {
        public static List<AggregateDataSegment> Aggregate(
            List<IAttributeDatum> attributeData,
            IEnumerable<Segment> networkSegments)
        {
            var aggregateDataSegments = new List<AggregateDataSegment>();

            // Copy the network segments into a new list of AggregateDataSegments
            foreach (var networkSegment in networkSegments)
            {
                aggregateDataSegments.Add(new AggregateDataSegment(networkSegment));
            }

            foreach (var datum in attributeData)
            {
                AggregateDataSegment matchingLocationSegment =
                    aggregateDataSegments.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Segment.Location));

                if (matchingLocationSegment != null)
                {
                    // Add the datum to the aggregation data segment
                    matchingLocationSegment.AddDatum(datum);
                }
                else
                {
                    // TODO: No matching segment for the current data. What do we do?
                }
            }
            return aggregateDataSegments;
        }
    }
}
