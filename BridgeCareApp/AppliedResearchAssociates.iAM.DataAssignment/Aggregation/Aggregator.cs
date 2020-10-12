using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public static class Aggregator
    {
        public static void AssignAttributeDataToMaintainableAsset(
            IEnumerable<IAttributeDatum> attributeData,
            IEnumerable<MaintainableAsset> maintainableAssets)
        {
            foreach (var datum in attributeData)
            {
                MaintainableAsset matchingLocationSegment =
                    maintainableAssets.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Location));

                if (matchingLocationSegment != null)
                {
                    matchingLocationSegment.AssignedData.Add(datum);
                }
                else
                {
                    // TODO: No matching segment for the current data. What do we do?
                }
            }
        }
    }
}
