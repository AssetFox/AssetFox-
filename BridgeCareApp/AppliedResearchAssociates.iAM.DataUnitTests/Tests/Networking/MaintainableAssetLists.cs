using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;


namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
{
    public static class MaintainableAssetLists
    {
        public static List<MaintainableAsset> SingleInNetwork(Guid networkId, string keyAttributeName, Guid? assetId = null, SectionLocation? location = null)
        {
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId, location);
            var list = new List<MaintainableAsset> { asset };
            return list;
        }
    }
}
