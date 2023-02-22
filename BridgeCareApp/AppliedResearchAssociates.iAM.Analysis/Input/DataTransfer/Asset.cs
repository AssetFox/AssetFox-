﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class Asset
    {
        public string ID { get; set; }

        public List<AttributeValueHistory<double>> NumberAttributeHistories { get; set; }

        public string SpatialWeightExpression { get; set; }

        public List<AttributeValueHistory<string>> TextAttributeHistories { get; set; }
    }
}
