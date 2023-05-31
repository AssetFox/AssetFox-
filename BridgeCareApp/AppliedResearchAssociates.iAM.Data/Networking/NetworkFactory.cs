using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data.Networking
{
    public static class NetworkFactory
    {
        /// <summary>
        ///     Use of this function to create a segmented network assumes that the attribute data
        ///     provided matches the desired pavement management sections.
        ///
        ///     The segments returned store a single attribute datum for management of "minimum"
        ///     section sizes. Miminum section sizes are anticipated to be a future enhancement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeData"></param>
        /// <returns></returns>
        public static Network CreateNetworkFromAttributeDataRecords(IEnumerable<IAttributeDatum> attributeData, string defaultEquation)
        {
            var anyDuplicate = attributeData.GroupBy(datum => datum.Location.LocationIdentifier).Any(g => g.Count() > 1);
            if(anyDuplicate)
            {
                attributeData = attributeData.GroupBy(datum => datum.Location.LocationIdentifier).Select(g => g.First());
            }
            var networkId = Guid.NewGuid();
            var maintenanceAssets = (from attributeDatum in attributeData
                    let maintenanceAsset = new MaintainableAsset(Guid.NewGuid(), networkId, attributeDatum.Location, defaultEquation)
                    select maintenanceAsset).ToList();
            return new Network(maintenanceAssets, networkId);
        }
    }
}
