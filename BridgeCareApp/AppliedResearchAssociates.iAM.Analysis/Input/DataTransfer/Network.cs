using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class Network
{
    public AttributeSystem AttributeSystem { get; set; } = new();

    public List<MaintainableAsset> MaintainableAssets { get; set; } = new();

    public string Name { get; set; }
}
