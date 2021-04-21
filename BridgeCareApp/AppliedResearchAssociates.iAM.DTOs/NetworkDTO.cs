using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class NetworkDTO : BaseDTO
    {
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string Status { get; set; }

        public BenefitQuantifierDTO BenefitQuantifier { get; set; }
    }
}
