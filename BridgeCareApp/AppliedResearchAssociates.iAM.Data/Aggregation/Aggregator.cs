using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data.Attributes;
using OfficeOpenXml.FormulaParsing.Logging;
using System;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public static class Aggregator
    {
        public static event EventHandler<AggregationLogEventArgs> AggregationLog;

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
        //private static void OnLog(AggregationLogEventArgs e) => AggregationLog?.Invoke(this, e);
    }
}
