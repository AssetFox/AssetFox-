using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class MaintainableAssetQueryDTO
    {
        public Guid AssetId { get; set; }
        public Dictionary<AttributeDTO, string> AssetProperties { get; set; }
    }
}
