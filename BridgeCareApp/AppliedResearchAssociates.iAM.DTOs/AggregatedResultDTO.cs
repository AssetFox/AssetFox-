using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class AggregatedResultDTO
    {
        public Guid MaintainableAssetId { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
        public AbbreviatedAttributeDTO Attribute { get; set; }
    }
}
