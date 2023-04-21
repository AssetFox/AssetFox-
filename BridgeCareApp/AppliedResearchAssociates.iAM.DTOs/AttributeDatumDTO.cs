using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class AttributeDatumDTO: BaseDTO
    {
        public string Attribute { get; set; }

        public string TextValue { get; set; }

        public double? NumericValue { get; set; }

        public Guid MaintainableAssetId { get; set; }
    }
}
