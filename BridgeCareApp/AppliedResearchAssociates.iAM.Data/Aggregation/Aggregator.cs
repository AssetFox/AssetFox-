using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
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
