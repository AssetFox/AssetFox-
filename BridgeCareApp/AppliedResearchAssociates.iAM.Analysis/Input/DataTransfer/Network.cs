using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class Network
    {
        public AttributeSystem AttributeSystem { get; set; }

        public List<MaintainableAsset> MaintainableAssets { get; set; }

        public string Name { get; set; }
    }
}
