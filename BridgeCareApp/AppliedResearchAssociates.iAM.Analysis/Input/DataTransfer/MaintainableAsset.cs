using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class MaintainableAsset
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public List<AttributeValueHistory<double>> NumberAttributeHistories { get; set; }

        public string SpatialWeightExpression { get; set; }

        public List<AttributeValueHistory<string>> TextAttributeHistories { get; set; }
    }
}
