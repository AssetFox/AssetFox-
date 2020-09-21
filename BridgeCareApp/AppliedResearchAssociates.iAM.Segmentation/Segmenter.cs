using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public static class Segmenter
    {
        /// <summary>
        ///     Use of this function to create a segmented network assumes that
        ///     the attribute data provided matches the desired pavement
        ///     management sections.
        ///
        ///     The segments returned store a single attribute datum for
        ///     management of "minimum" section sizes. Miminum section sizes are
        ///     anticipated to be a future enhancement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeData"></param>
        /// <returns></returns>
        public static Network CreateNetworkFromAttributeDataRecords<T>(IEnumerable<IAttributeDatum> attributeData)
        {
            var segments = (from attributeDatum in attributeData
                    let segment = new Segment(attributeDatum.Location, attributeDatum)
                    select segment)
                    .ToList();
            return new Network(segments, Guid.NewGuid());
        }
    }
}
